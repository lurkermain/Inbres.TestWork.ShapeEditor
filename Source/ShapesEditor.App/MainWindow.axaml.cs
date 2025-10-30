using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DynamicData;
using System;
using System.Collections.Generic;
using System.IO;

//using System.IO;
using System.Linq;
using System.Text.Json;
using static ShapesEditor.App.Models.Models;

namespace ShapesEditor.App
{
	public partial class MainWindow : Window
	{
		private Canvas? _canvas;
		private ComboBox? _toolSelector;
		private Button? _finishCurveBtn, _saveBtn, _loadBtn, _clearBtn;

		// models (serializable)
		private List<ShapeModel> _models = new();
		// visuals map
		private Dictionary<string, Control> _visuals = new();

		// drawing state
		private bool _isDrawingRect;
		private Point _start;
		private Rectangle? _rectVisual;
		private RectangleModel? _rectModel;

		private BezierModel? _activeBezierModel;
		private Avalonia.Controls.Shapes.Path? _bezierVisual;
		private Point _mousePreview;

		public MainWindow() { InitializeComponent(); InitRefs(); }

		private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		private void InitRefs()
		{
			_canvas = this.FindControl<Canvas>("DrawingCanvas");
			_toolSelector = this.FindControl<ComboBox>("ToolSelector");
			_finishCurveBtn = this.FindControl<Button>("FinishCurveButton");
			_saveBtn = this.FindControl<Button>("SaveButton");
			_loadBtn = this.FindControl<Button>("LoadButton");
			_clearBtn = this.FindControl<Button>("ClearButton");

			if (_canvas == null) throw new InvalidOperationException("Canvas missing");

			_canvas.PointerPressed += Canvas_PointerPressed;
			_canvas.PointerMoved += Canvas_PointerMoved;
			_canvas.PointerReleased += Canvas_PointerReleased;

			_finishCurveBtn.Click += FinishCurveButton_Click;
			_saveBtn.Click += SaveButton_Click;
			_loadBtn.Click += LoadButton_Click;
			_clearBtn.Click += ClearButton_Click;

			RedrawAll();
		}

		// --- input handlers ---
		private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (_canvas == null) return;
			var pt = e.GetPosition(_canvas);
			if (!e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed) return;

			string tool = (_toolSelector?.SelectedItem as string) ?? "Прямоугольник";

			if (tool == "Прямоугольник")
			{
				_isDrawingRect = true;
				_start = pt;
				_rectModel = new RectangleModel { Id = Guid.NewGuid().ToString(), X = pt.X, Y = pt.Y, Width = 0, Height = 0, Stroke = "#FF007ACC", StrokeThickness = 2 };
				_rectVisual = new Rectangle { Stroke = new SolidColorBrush(Color.Parse(_rectModel.Stroke)), StrokeThickness = _rectModel.StrokeThickness, Fill = Brushes.Transparent };
				Canvas.SetLeft(_rectVisual, pt.X); Canvas.SetTop(_rectVisual, pt.Y);
				_canvas.Children.Add(_rectVisual);
			}
			else // Cubic Bezier tool
			{
				if (_activeBezierModel == null)
				{
					_activeBezierModel = new BezierModel { Id = Guid.NewGuid().ToString(), Stroke = "#FF2E8B57", StrokeThickness = 2 };
					_activeBezierModel.Anchors.Add(SPoint.FromAvalonia(pt));
					_bezierVisual = CreatePathFromBezier(_activeBezierModel);
					_canvas.Children.Add(_bezierVisual);
				}
				else
				{
					// add new anchor (endpoint) and compute simple control pair
					var prev = _activeBezierModel.Anchors.Last();
					var anchor = SPoint.FromAvalonia(pt);
					var dx = anchor.X - prev.X; var dy = anchor.Y - prev.Y;
					var c1 = new SPoint(prev.X + dx / 3.0, prev.Y + dy / 3.0);
					var c2 = new SPoint(prev.X + dx * 2.0 / 3.0, prev.Y + dy * 2.0 / 3.0);
					_activeBezierModel.Controls.Add(new ControlPair { C1 = c1, C2 = c2 });
					_activeBezierModel.Anchors.Add(anchor);
					UpdateBezierVisual(_bezierVisual, _activeBezierModel);
				}
			}
		}

		private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
		{
			if (_canvas == null) return;
			_mousePreview = e.GetPosition(_canvas);

			if (_isDrawingRect && _rectVisual != null && _rectModel != null)
			{
				double x = Math.Min(_start.X, _mousePreview.X);
				double y = Math.Min(_start.Y, _mousePreview.Y);
				double w = Math.Abs(_mousePreview.X - _start.X);
				double h = Math.Abs(_mousePreview.Y - _start.Y);
				Canvas.SetLeft(_rectVisual, x); Canvas.SetTop(_rectVisual, y);
				_rectVisual.Width = w; _rectVisual.Height = h;
				_rectModel.X = x; _rectModel.Y = y; _rectModel.Width = w; _rectModel.Height = h;
			}
			else if (_activeBezierModel != null && _bezierVisual != null)
			{
				// preview last segment to current mouse pos
				var prev = _activeBezierModel.Anchors.Last();
				var anchor = SPoint.FromAvalonia(_mousePreview);
				var dx = anchor.X - prev.X; var dy = anchor.Y - prev.Y;
				var c1 = new SPoint(prev.X + dx / 3.0, prev.Y + dy / 3.0);
				var c2 = new SPoint(prev.X + dx * 2.0 / 3.0, prev.Y + dy * 2.0 / 3.0);
				UpdateBezierVisual(_bezierVisual, _activeBezierModel, previewControl: new ControlPair { C1 = c1, C2 = c2 }, previewAnchor: anchor);
			}
		}

		private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			if (_isDrawingRect && _rectModel != null && _rectVisual != null)
			{
				_models.Add(_rectModel);
				_visuals[_rectModel.Id] = _rectVisual;
				_rectModel = null; _rectVisual = null; _isDrawingRect = false;
			}
		}

		// --- buttons ---
		private void FinishCurveButton_Click(object? s, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (_activeBezierModel != null && _bezierVisual != null)
			{
				_models.Add(_activeBezierModel);
				_visuals[_activeBezierModel.Id] = _bezierVisual;
				_activeBezierModel = null; _bezierVisual = null;
			}
		}

		private async void SaveButton_Click(object? s, Avalonia.Interactivity.RoutedEventArgs e)
		{
			try
			{
				var opts = new JsonSerializerOptions { WriteIndented = true };
				opts.Converters.Add(new ShapeModelJsonConverter());
				var json = JsonSerializer.Serialize(_models, opts);
				await File.WriteAllTextAsync("shapes.json", json);
				await SimpleDialog.Show(this, "Saved to shapes.json");
			}
			catch (Exception ex) { await SimpleDialog.Show(this, "Save failed: " + ex.Message); }
		}

		private async void LoadButton_Click(object? s, Avalonia.Interactivity.RoutedEventArgs e)
		{
			try
			{
				if (!File.Exists("shapes.json")) { await SimpleDialog.Show(this, "shapes.json not found"); return; }
				var txt = await File.ReadAllTextAsync("shapes.json");
				var opts = new JsonSerializerOptions(); opts.Converters.Add(new ShapeModelJsonConverter());
				var list = JsonSerializer.Deserialize<List<ShapeModel>>(txt, opts);
				if (list != null) { _models = list; RedrawAll(); }
			}
			catch (Exception ex) { await SimpleDialog.Show(this, "Load failed: " + ex.Message); }
		}

		private void ClearButton_Click(object? s, Avalonia.Interactivity.RoutedEventArgs e)
		{
			_models.Clear(); _visuals.Clear(); _canvas?.Children.Clear();
			_activeBezierModel = null; _bezierVisual = null; _rectModel = null; _rectVisual = null; _isDrawingRect = false;
		}

		// --- visuals helpers ---
		private Avalonia.Controls.Shapes.Path CreatePathFromBezier(BezierModel model)
		{
			var geom = new PathGeometry();
			if (model.Anchors.Count > 0)
			{
				var fig = new PathFigure { StartPoint = model.Anchors[0].ToAvalonia() };
				for (int i = 0; i < model.Controls.Count; i++)
				{
					var cp = model.Controls[i];
					var seg = new BezierSegment { Point1 = cp.C1.ToAvalonia(), Point2 = cp.C2.ToAvalonia(), Point3 = model.Anchors[i + 1].ToAvalonia() };
					fig.Segments.Add(seg);
				}
				geom.Figures.Add(fig);
			}
			return new Avalonia.Controls.Shapes.Path { Data = geom, Stroke = new SolidColorBrush(Color.Parse(model.Stroke)), StrokeThickness = model.StrokeThickness, Fill = Brushes.Transparent };
		}

		private void UpdateBezierVisual(Avalonia.Controls.Shapes.Path? path, BezierModel model, ControlPair? previewControl = null, SPoint? previewAnchor = null)
		{
			if (path == null) return;
			var geom = new PathGeometry();
			if (model.Anchors.Count > 0)
			{
				var fig = new PathFigure { StartPoint = model.Anchors[0].ToAvalonia() };
				for (int i = 0; i < model.Controls.Count; i++)
				{
					var cp = model.Controls[i];
					fig.Segments.Add(new BezierSegment { Point1 = cp.C1.ToAvalonia(), Point2 = cp.C2.ToAvalonia(), Point3 = model.Anchors[i + 1].ToAvalonia() });
				}
				if (previewControl != null && previewAnchor != null)
				{
					fig.Segments.Add(new BezierSegment { Point1 = previewControl.C1.ToAvalonia(), Point2 = previewControl.C2.ToAvalonia(), Point3 = previewAnchor.Value.ToAvalonia() });
				}
				geom.Figures.Add(fig);
			}
			path.Data = geom;
			path.Stroke = new SolidColorBrush(Color.Parse(model.Stroke));
			path.StrokeThickness = model.StrokeThickness;
		}

		private Rectangle CreateRectangleVisual(RectangleModel model)
		{
			var r = new Rectangle { Width = model.Width, Height = model.Height, Stroke = new SolidColorBrush(Color.Parse(model.Stroke)), StrokeThickness = model.StrokeThickness, Fill = Brushes.Transparent };
			Canvas.SetLeft(r, model.X); Canvas.SetTop(r, model.Y);
			return r;
		}

		private void RedrawAll()
		{
			_canvas?.Children.Clear();
			_visuals.Clear();
			foreach (var m in _models)
			{
				if (m is RectangleModel rm)
				{
					var v = CreateRectangleVisual(rm);
					_visuals[rm.Id] = v; _canvas?.Children.Add(v);
				}
				else if (m is BezierModel bm)
				{
					var p = CreatePathFromBezier(bm);
					_visuals[bm.Id] = p; _canvas?.Children.Add(p);
				}
			}
		}
	}

	// very small dialog helper
	static class SimpleDialog
	{
		public static async System.Threading.Tasks.Task Show(Window owner, string text)
		{
			var d = new Window { Width = 360, Height = 120, Title = "Info", WindowStartupLocation = WindowStartupLocation.CenterOwner };
			var panel = new StackPanel { Margin = new Thickness(10) };
			panel.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
			var ok = new Button { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Width = 80 };
			ok.Click += (_, _) => d.Close();
			panel.Children.Add(ok);
			d.Content = panel;
			await d.ShowDialog(owner);
		}
	}
}

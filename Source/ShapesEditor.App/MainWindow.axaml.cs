using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace ShapesEditor.App
{
	public partial class MainWindow : Window
	{
		private bool _isDrawing = false;
		private Point _start;
		private Rectangle? _currentRect;
		private Canvas? _drawingCanvas;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
			_drawingCanvas = this.FindControl<Canvas>("DrawingCanvas");
			if (_drawingCanvas == null)
				throw new InvalidOperationException("DrawingCanvas not found in XAML. ”бедитесь, что в MainWindow.axaml есть Canvas с Name=\"DrawingCanvas\".");
		}

		private void DrawingCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (_drawingCanvas == null) return;

			var pressed = e.GetCurrentPoint(_drawingCanvas);
			if (!pressed.Properties.IsLeftButtonPressed) return;

			_isDrawing = true;
			_start = e.GetPosition(_drawingCanvas);

			_currentRect = new Rectangle
			{
				Stroke = Brushes.Black,
				StrokeThickness = 1,
				Fill = Brushes.Transparent
			};

			Canvas.SetLeft(_currentRect, _start.X);
			Canvas.SetTop(_currentRect, _start.Y);

			_drawingCanvas.Children.Add(_currentRect);

			// ”Ѕ–јЌ захват указател€ Ч Avalonia не имеет CapturePointer на Canvas
		}

		private void DrawingCanvas_PointerMoved(object? sender, PointerEventArgs e)
		{
			if (_drawingCanvas == null) return;
			if (!_isDrawing || _currentRect == null) return;

			var pos = e.GetPosition(_drawingCanvas);

			double x = Math.Min(pos.X, _start.X);
			double y = Math.Min(pos.Y, _start.Y);
			double w = Math.Abs(pos.X - _start.X);
			double h = Math.Abs(pos.Y - _start.Y);

			Canvas.SetLeft(_currentRect, x);
			Canvas.SetTop(_currentRect, y);
			_currentRect.Width = w;
			_currentRect.Height = h;
		}

		private void DrawingCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			if (_drawingCanvas == null) return;
			if (!_isDrawing) return;

			_isDrawing = false;
			_currentRect = null;

			// ”Ѕ–јЌ ReleasePointerCapture Ч если хотите захват/освобождение,
			// см. альтернативный пример ниже (зависит от версии Avalonia)
		}

		private void ClearButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (_drawingCanvas == null) return;
			_drawingCanvas.Children.Clear();
		}
	}
}
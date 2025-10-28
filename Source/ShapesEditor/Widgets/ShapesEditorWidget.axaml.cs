using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Media;
using System;
using ShapesEditor.App.Widgets;
using Avalonia.Controls.Shapes;

namespace ShapesEditor.App;

public partial class ShapesEditorWidget : UserControl
{
	public ShapesEditorWidgetModel VM => (ShapesEditorWidgetModel)DataContext!;
	private bool _isDragging = false;
	private Point _dragStart;

	public ShapesEditorWidget()
	{
		this.InitializeComponent();
	}

	private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

	private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		var p = e.GetPosition(this);
		// Forward to VM. Handles creation of curve points or start of rectangle resizing.
		VM.CanvasClicked(p);
		UpdatePreview();
	}

	private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
	{
		var p = e.GetPosition(this);
		// If resizing primitives during creation — update width/height
		UpdatePreview();
	}

	private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		UpdatePreview();
	}

	private void UpdatePreview()
	{
		var preview = this.FindControl<Path>("PreviewPath");
		if (VM.CreatingKind == ShapesEditor.App.Models.ShapeKind.QuadraticBezier && VM.TempQuadraticModel != null)
		{
			var data = VM.TempQuadraticModel.ToPathData();
			if (!string.IsNullOrEmpty(data))
				preview.Data = Geometry.Parse(data);
		}
		else preview.Data = null;
	}
}
using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Models
{
	public enum ShapeKind { Rectangle, Ellipse, Triangle, QuadraticBezier, CubicBezier }
	public abstract class ShapeBase
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public ShapeKind Kind { get; set; }
		public Point Position { get; set; } // anchor / offset
		public Color Stroke { get; set; } = Colors.Black;
		public double StrokeThickness { get; set; } = 1.0;
		public Color Fill { get; set; } = Colors.Transparent;

		// Each shape provides its control points (serializable)
		public abstract List<Point> ControlPoints { get; }

		// Optional helper to convert to a Path Data string (SVG-like)
		public abstract string ToPathData();
	}
}

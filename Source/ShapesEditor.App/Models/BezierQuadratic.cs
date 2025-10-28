using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Models
{
	public class BezierQuadratic : ShapeBase
	{
		// List of points: start, [intermediate control points pairs?] ... end
		// For quadratic bezier segments with multiple segments, we'll store sequence as:
		// startPoint, controlPoint1, endPoint1, controlPoint2, endPoint2, ...
		public List<Point> Points { get; set; } = new List<Point>();

		public override List<Point> ControlPoints => new List<Point>(Points);

		public BezierQuadratic()
		{
			Kind = ShapeKind.QuadraticBezier;
		}

		public override string ToPathData()
		{
			if (Points.Count < 3) return string.Empty;
			// build: M startX startY Q cx cy x y [Q cx cy x y]...
			var sb = new System.Text.StringBuilder();
			sb.Append($"M {Points[0].X} {Points[0].Y}");
			for (int i = 1; i + 1 < Points.Count; i += 2)
			{
				var c = Points[i];
				var e = Points[i + 1];
				sb.Append($" Q {c.X} {c.Y} {e.X} {e.Y}");
			}
			return sb.ToString();
		}
	}
}

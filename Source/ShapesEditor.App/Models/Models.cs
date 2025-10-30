using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Models
{
	public class Models
	{
		public abstract class ShapeModel { public string Id { get; set; } = System.Guid.NewGuid().ToString(); public string Type { get; set; } = ""; }

		public class RectangleModel : ShapeModel
		{
			public RectangleModel() { Type = "Rectangle"; }
			public double X { get; set; }
			public double Y { get; set; }
			public double Width { get; set; }
			public double Height { get; set; }
			public string Stroke { get; set; } = "#FF000000"; public double StrokeThickness { get; set; } = 1;
			public string Fill { get; set; } = "#00FFFFFF";
		}

		public class BezierModel : ShapeModel
		{
			public BezierModel() { Type = "CubicBezier"; Anchors = new List<SPoint>(); Controls = new List<ControlPair>(); }
			public List<SPoint> Anchors { get; set; }
			public List<ControlPair> Controls { get; set; }
			public string Stroke { get; set; } = "#FF000000"; public double StrokeThickness { get; set; } = 2;
		}

		public class ControlPair { public SPoint C1 { get; set; } public SPoint C2 { get; set; } }

		public struct SPoint
		{
			public double X { get; set; }
			public double Y { get; set; }

			// Добавляем конструктор
			public SPoint(double x, double y)
			{
				X = x;
				Y = y;
			}

			public static SPoint FromAvalonia(Point p) => new SPoint(p.X, p.Y);
			public Point ToAvalonia() => new Point(X, Y);
		}

	}
}

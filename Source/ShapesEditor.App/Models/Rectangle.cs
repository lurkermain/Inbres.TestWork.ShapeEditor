using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Models
{
	public class Rectangle : ShapeBase
	{
		public double Width { get; set; } = 100;
		public double Height { get; set; } = 60;

		// For serialization/deserialization use Position + Width/Height; ControlPoints computed
		public override List<Point> ControlPoints
			=> new List<Point> {
				new Point(Position.X, Position.Y),
				new Point(Position.X + Width, Position.Y),
				new Point(Position.X + Width, Position.Y + Height),
				new Point(Position.X, Position.Y + Height)
			};

		public Rectangle()
		{
			Kind = ShapeKind.Rectangle;
		}

		public override string ToPathData()
		{
			// simple rect path: M x y L x+w y L x+w y+h L x y+h Z
			var x = Position.X;
			var y = Position.Y;
			return $"M {x} {y} L {x + Width} {y} L {x + Width} {y + Height} L {x} {y + Height} Z";
		}
	}
}

using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Models
{
	/// <summary>
	/// Модель для кубической кривой Безье.
	/// Формат Points:
	///  - Points[0] = стартовая точка (M)
	///  - далее каждая сегментная тройка: control1, control2, end
	///    т.е. последовательность: start, c1, c2, end, c1, c2, end, ...
	/// </summary>
	public class BezierCubic : ShapeBase
	{
		public List<Point> Points { get; set; } = new List<Point>();

		public BezierCubic()
		{
			Kind = ShapeKind.CubicBezier;
		}

		public override List<Point> ControlPoints => new List<Point>(Points);

		public override string ToPathData()
		{
			if (Points == null || Points.Count == 0)
				return string.Empty;

			// Минимум для одного сегмента: start + c1 + c2 + end => 4 точ.
			if (Points.Count < 4)
				return string.Empty;

			var sb = new System.Text.StringBuilder();
			// M startX startY
			var start = Points[0];
			sb.Append($"M {start.X} {start.Y}");

			// начинаем с индекса 1 и читаем по 3 (c1,c2,end)
			for (int i = 1; i + 2 < Points.Count; i += 3)
			{
				var c1 = Points[i];
				var c2 = Points[i + 1];
				var e = Points[i + 2];
				sb.Append($" C {c1.X} {c1.Y} {c2.X} {c2.Y} {e.X} {e.Y}");
			}

			return sb.ToString();
		}
	}
}

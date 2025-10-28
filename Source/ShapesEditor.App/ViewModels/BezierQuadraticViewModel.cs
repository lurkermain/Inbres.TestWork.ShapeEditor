using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using ShapesEditor.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.ViewModels
{
	public class BezierQuadraticViewModel : ShapeViewModelBase
	{
		private BezierQuadratic Model => (BezierQuadratic)_model;

		// Observable collection used by the View to bind markers etc.
		public ObservableCollection<Point> Points { get; }

		public BezierQuadraticViewModel(BezierQuadratic model) : base(model)
		{
			// initialize collection from model
			Points = new ObservableCollection<Point>(model.Points ?? Enumerable.Empty<Point>());

			// subscribe to changes so we update model and geometry
			Points.CollectionChanged += OnPointsCollectionChanged;
		}

		private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			// copy back to model (simple approach)
			Model.Points = Points.ToList();
			// notify bindings that geometry/controlpoints changed
			this.RaisePropertyChanged(nameof(PathGeometry));
			this.RaisePropertyChanged(nameof(ControlPoints));
		}

		// Convenience methods for editing points
		public void AddPoint(Point p)
		{
			Points.Add(p);
			// OnPointsCollectionChanged will update model and raise notifications
		}

		public void InsertPoint(int index, Point p)
		{
			Points.Insert(index, p);
		}

		public void MovePoint(int index, Point newPosition)
		{
			if (index < 0 || index >= Points.Count) return;
			// replace item to trigger CollectionChanged
			Points[index] = newPosition;
		}

		public void RemovePointAt(int index)
		{
			if (index < 0 || index >= Points.Count) return;
			Points.RemoveAt(index);
		}

		public override Geometry PathGeometry
		{
			get
			{
				var pathData = Model.ToPathData();
				if (string.IsNullOrWhiteSpace(pathData))
					return Geometry.Parse(string.Empty); // empty geometry
				try
				{
					return Geometry.Parse(pathData);
				}
				catch (Exception)
				{
					// Fallback: try to build StreamGeometry manually (simple single-segment support)
					return BuildFallbackGeometry();
				}
			}
		}

		private Geometry BuildFallbackGeometry()
		{
			if (Points.Count < 2) return Geometry.Parse(string.Empty);

			var sg = new StreamGeometry();
			using (var ctx = sg.Open())
			{
				ctx.BeginFigure(Points[0], isFilled: false /*, isClosed: false*/ );

				// simplest fallback: connect points with quadratic segments (every control+end pair)
				if (Points.Count >= 3)
				{
					for (int i = 1; i + 1 < Points.Count; i += 2)
					{
						var c = Points[i];
						var e = Points[i + 1];
						// StreamGeometryContext doesn't have direct QuadraticTo method in Avalonia <=11,
						// So approximate quadratic with cubic conversion or simple line fallback.
						// We'll fallback to a polyline to keep UI responsive.
						ctx.LineTo(c);
						ctx.LineTo(e);
					}
				}
				else
				{
					// just line to last
					ctx.LineTo(Points[^1]);
				}
			}
			return sg;
		}

		public override System.Collections.Generic.List<Point> ControlPoints
			=> Points.ToList();
	}
}


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
	public class BezierCubicViewModel : ShapeViewModelBase
	{
		private BezierCubic Model => (BezierCubic)_model;

		public ObservableCollection<Point> Points { get; }

		public BezierCubicViewModel(BezierCubic model) : base(model)
		{
			Points = new ObservableCollection<Point>(model.Points ?? Enumerable.Empty<Point>());
			Points.CollectionChanged += OnPointsCollectionChanged;
		}

		private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			Model.Points = Points.ToList();
			this.RaisePropertyChanged(nameof(PathGeometry));
			this.RaisePropertyChanged(nameof(ControlPoints));
		}

		public void AddPoint(Point p) => Points.Add(p);

		public void InsertPoint(int index, Point p) => Points.Insert(index, p);

		public void MovePoint(int index, Point newPosition)
		{
			if (index < 0 || index >= Points.Count) return;
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
					return Geometry.Parse(string.Empty);
				try
				{
					return Geometry.Parse(pathData);
				}
				catch
				{
					return Geometry.Parse(string.Empty);
				}
			}
		}

		public override System.Collections.Generic.List<Point> ControlPoints => Points.ToList();
	}
}

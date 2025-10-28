using Avalonia;
using Avalonia.Media;
using ReactiveUI;
using ShapesEditor.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.ViewModels
{
	public abstract class ShapeViewModelBase : ReactiveObject
	{
		protected readonly ShapeBase _model;
		public string Id => _model.Id;
		public ShapeKind Kind => _model.Kind;

		public Color Stroke
		{
			get => _model.Stroke;
			set
			{
				_model.Stroke = value;
				this.RaisePropertyChanged(nameof(Stroke));
			}
		}

		public double StrokeThickness
		{
			get => _model.StrokeThickness;
			set
			{
				_model.StrokeThickness = value;
				this.RaisePropertyChanged(nameof(StrokeThickness));
			}
		}

		public Color Fill
		{
			get => _model.Fill;
			set
			{
				_model.Fill = value;
				this.RaisePropertyChanged(nameof(Fill));
			}
		}

		public abstract Geometry PathGeometry { get; }
		public abstract List<Point> ControlPoints { get; }

		protected ShapeViewModelBase(ShapeBase model)
		{
			_model = model;
		}
	}
}

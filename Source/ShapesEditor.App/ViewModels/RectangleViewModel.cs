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
	public class RectangleViewModel : ShapeViewModelBase
	{
		private Rectangle Model => (Rectangle)_model;
		public RectangleViewModel(Rectangle model) : base(model) { }

		public double Width
		{
			get => Model.Width;
			set { Model.Width = value; this.RaisePropertyChanged(nameof(Width)); this.RaisePropertyChanged(nameof(PathGeometry)); }
		}

		public double Height
		{
			get => Model.Height;
			set { Model.Height = value; this.RaisePropertyChanged(nameof(Height)); this.RaisePropertyChanged(nameof(PathGeometry)); }
		}

		public override Geometry PathGeometry
			=> Geometry.Parse(Model.ToPathData());

		public override List<Point> ControlPoints => Model.ControlPoints;
	}
}

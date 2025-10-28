using Avalonia;
using ReactiveUI;
using ShapesEditor.App.Models;
using ShapesEditor.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App.Widgets
{
	public class ShapesEditorWidgetModel : ReactiveObject
	{
		public ObservableCollection<ShapeViewModelBase> Shapes { get; } = new();

		// Текущий режим создания: Rectangle, Ellipse, Triangle, Quadratic, Cubic, None
		public ShapeKind? CreatingKind { get; set; }

		// Для кривых: временный модель/VM пока пользователь добавляет точки
		public BezierQuadratic? TempQuadraticModel { get; set; }
		public BezierCubic? TempCubicModel { get; set; }

		public ReactiveCommand<Unit, Unit> AddRectangleCommand { get; }
		public ReactiveCommand<Unit, Unit> StartQuadraticCommand { get; }
		public ReactiveCommand<Unit, Unit> CommitTempShapeCommand { get; }
		public ReactiveCommand<Unit, Unit> SerializeCommand { get; }
		public ReactiveCommand<Unit, Unit> DeserializeCommand { get; }

		public ShapesEditorWidgetModel()
		{
			AddRectangleCommand = ReactiveCommand.Create(() =>
			{
				var model = new Rectangle { Position = new Point(20, 20), Width = 120, Height = 80 };
				var vm = new RectangleViewModel(model);
				Shapes.Add(vm);
			});

			StartQuadraticCommand = ReactiveCommand.Create(() =>
			{
				CreatingKind = ShapeKind.QuadraticBezier;
				TempQuadraticModel = new BezierQuadratic();
				// At creation start, user will click to add first points — here we only prepare model
			});

			CommitTempShapeCommand = ReactiveCommand.Create(() =>
			{
				if (CreatingKind == ShapeKind.QuadraticBezier && TempQuadraticModel != null && TempQuadraticModel.Points.Count >= 3)
				{
					var vm = new BezierQuadraticViewModel(TempQuadraticModel);
					Shapes.Add(vm);
					TempQuadraticModel = null;
					CreatingKind = null;
				}
				// similar for cubic...
			});

			SerializeCommand = ReactiveCommand.Create(() => {
				// реализовано в SerializationHelpers — ниже пример
			});

			DeserializeCommand = ReactiveCommand.Create(() => {
				// ...
			});
		}

		// Метод, который вызывается при клике по холсту (хост вызовет его)
		public void CanvasClicked(Point p)
		{
			if (CreatingKind == ShapeKind.QuadraticBezier && TempQuadraticModel != null)
			{
				TempQuadraticModel.Points.Add(p);
				// если получили старт+control+end (3 точки) — можно либо автоматически коммитить, либо ждать user commit
			}
			// Прочие режимы: при создании примитива возможно drag для размера — см. View (pointer events)
		}

	}
}

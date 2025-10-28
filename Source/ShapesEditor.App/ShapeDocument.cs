using ShapesEditor.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesEditor.App
{
	public class ShapeDocument
	{
		public List<ShapeBase> Shapes { get; set; } = new();
	}
}

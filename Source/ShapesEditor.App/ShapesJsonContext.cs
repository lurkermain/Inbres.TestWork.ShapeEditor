using ShapesEditor.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShapesEditor.App
{
	/*[JsonSerializable(typeof(ShapeDocument))]
	[JsonSerializable(typeof(Rectangle))]
	[JsonSerializable(typeof(Ellipse))]
	[JsonSerializable(typeof(Triangle))]
	[JsonSerializable(typeof(BezierQuadratic))]
	[JsonSerializable(typeof(BezierCubic))]
	public class ShapesJsonContext : JsonSerializerContext
	{
		*//*// Сериализация
		var doc = new ShapeDocument { Shape };
		string json = JsonSerializer.Serialize(doc, ShapesJsonContext.Default.ShapesDocument);

		// Десериализация
		var loaded = JsonSerializer.Deserialize(json, ShapesJsonContext.Default.ShapesDocument);*//*
	}*/
}

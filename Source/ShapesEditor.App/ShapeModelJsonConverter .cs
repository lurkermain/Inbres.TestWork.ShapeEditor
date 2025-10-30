using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ShapesEditor.App.Models.Models;

namespace ShapesEditor.App
{
	public class ShapeModelJsonConverter : JsonConverter<ShapeModel>
	{
		public override ShapeModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);
			if (!doc.RootElement.TryGetProperty("Type", out var t)) return null;
			var type = t.GetString();
			return type switch
			{
				"Rectangle" => JsonSerializer.Deserialize<RectangleModel>(doc.RootElement.GetRawText(), options),
				"CubicBezier" => JsonSerializer.Deserialize<BezierModel>(doc.RootElement.GetRawText(), options),
				_ => null
			};
		}

		public override void Write(Utf8JsonWriter writer, ShapeModel value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case RectangleModel r: JsonSerializer.Serialize(writer, r, options); break;
				case BezierModel b: JsonSerializer.Serialize(writer, b, options); break;
				default: JsonSerializer.Serialize(writer, value, options); break;
			}
		}
	}
}

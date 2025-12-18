using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Foxbyte.Core
{
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color color = (Color)value;
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(color.r);
            writer.WritePropertyName("g");
            writer.WriteValue(color.g);
            writer.WritePropertyName("b");
            writer.WriteValue(color.b);
            writer.WritePropertyName("a");
            writer.WriteValue(color.a);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            float r = obj["r"].Value<float>();
            float g = obj["g"].Value<float>();
            float b = obj["b"].Value<float>();
            float a = obj["a"].Value<float>();
            return new Color(r, g, b, a);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
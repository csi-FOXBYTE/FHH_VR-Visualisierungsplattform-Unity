using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Foxbyte.Core
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            float x = obj["x"].Value<float>();
            float y = obj["y"].Value<float>();
            float z = obj["z"].Value<float>();
            return new Vector3(x, y, z);
        }
    }
}
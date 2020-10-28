using Newtonsoft.Json;
using System;
using UnityEngine;

namespace AlternativePlay.Models
{
    /// <summary>
    /// This class serializes and deserializes the UnityEngine.Vector3 class
    /// to and from JSON
    /// </summary>
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 value, bool hasvalue, JsonSerializer serializer)
        {
            float[] values = serializer.Deserialize<float[]>(reader);
            return new Vector3(values[0], values[1], values[2]);
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            float[] values = new float[3];

            values[0] = value.x;
            values[1] = value.y;
            values[2] = value.z;

            serializer.Serialize(writer, values);
        }
    }
}

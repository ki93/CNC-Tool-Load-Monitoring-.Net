using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HNInc.Communication.Library.Http
{
    class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string stringBytes = JsonSerializer.Deserialize<string>(ref reader);
            //byte[] value = JsonSerializer.Deserialize<byte[]>(ref reader);
            char[] charBytes = stringBytes.ToCharArray();
            int charBytesLength = charBytes.Length;
            byte[] value = new byte[charBytesLength];
            for (int i = 0; i < charBytesLength; i++)
            {
                value[i] = (byte)charBytes[i];
            }

            return value;
        }
        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var val in value)
            {
                writer.WriteNumberValue(val);
            }

            writer.WriteEndArray();
        }
    }
}

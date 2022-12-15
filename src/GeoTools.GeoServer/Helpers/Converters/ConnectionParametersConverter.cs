using GeoTools.GeoServer.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Helpers.Converters
{
    internal class ConnectionParametersConverter : JsonConverter<ConnectionParameters>
    {
        public override ConnectionParameters Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName != "entry")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var parameters = new List<ConnectionParameter>();

            reader.Read();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                string key = "";
                string value = "";

                for (int i = 0; i < 2; ++i)
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    var readerKey = reader.GetString();
                    if (readerKey == "@key")
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        key = reader.GetString();
                    }
                    else if (readerKey == "$")
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        value = reader.GetString();
                    }
                    else
                    {
                        throw new JsonException($"Unknown JSON key {readerKey}.");
                    }
                }

                parameters.Add(new ConnectionParameter(key, value));

                reader.Read();
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException();
                }

                reader.Read();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new ConnectionParameters(parameters);
        }

        public override void Write(Utf8JsonWriter writer, ConnectionParameters value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("entry");
            writer.WriteStartArray();
            if (value != null && value.ConnectionParameterList != null)
            {
                foreach (var parameter in value.ConnectionParameterList)
                {
                    writer.WriteStartObject();

                    writer.WriteString("@key", parameter.Key);
                    writer.WriteString("$", parameter.Value);

                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}

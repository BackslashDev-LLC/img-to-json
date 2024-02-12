using BackslashDev.LLMTools.ImgToJson.Attributes;
using System.Reflection;
using System.Text.Json.Nodes;

namespace BackslashDev.LLMTools.ImgToJson.Json
{
    public class SchemaGenerator
    {
        public static JsonObject GenerateJsonSchema<T>()
        {
            var schema = new JsonObject
            {
                ["$schema"] = "http://json-schema.org/draft-07/schema#",
                ["title"] = typeof(T).Name,
                ["type"] = "object",
                ["properties"] = new JsonObject()
            };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var propertySchema = GetPropertySchema(property);

                if (propertySchema != null)
                {
                    ((JsonObject)schema["properties"]).Add(property.Name, propertySchema);
                }
            }

            return schema;
        }

        private static string MapTypeToString(Type type)
        {
            if (type == typeof(string) || type == typeof(char))
            {
                return "string";
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(short))
            {
                return "integer";
            }
            else if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
            {
                return "number";
            }
            else if (type == typeof(bool))
            {
                return "boolean";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return "array";
            }
            else if (type.IsClass)
            {
                return "object";
            }

            throw new ArgumentException("Unsupported type received.", nameof(type));
        }

        private static JsonNode? GetPropertySchema(PropertyInfo property)
        {
            var ignoreAttribute = property.GetCustomAttribute<SchemaIgnoreAttribute>();
            if (ignoreAttribute != null)
            {
                return null;
            }

            var type = property.PropertyType;
            var strType = MapTypeToString(property.PropertyType);

            var propertySchema = new JsonObject { ["type"] = strType };
            
            var descriptionAttribute = property.GetCustomAttribute<JsonDescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                propertySchema["description"] = descriptionAttribute.Description;
            }

            var enumAttribute = property.GetCustomAttribute<JsonEnumAttribute>();
            if (enumAttribute != null)
            {
                var enumArray = new JsonArray();
                foreach (var enumValue in enumAttribute.Enum)
                {
                    enumArray.Add(enumValue);
                }
                propertySchema["enum"] = enumArray;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = type.GetGenericArguments()[0];
                var itemStrType = MapTypeToString(itemType);

                if (itemStrType == "object")
                {
                    propertySchema["items"] = new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() };
                    var arrayTypeProperties = itemType.GetProperties();
                    foreach (var prop in arrayTypeProperties)
                    {
                        var propSchema = GetPropertySchema(prop);
                        if (propSchema != null)
                        {
                            ((JsonObject)propertySchema["items"]["properties"]).Add(prop.Name, propSchema);
                        }
                    }
                }
                else
                {
                    propertySchema["items"] = new JsonObject { ["type"] = itemStrType };
                }
            }
            else if (strType == "object")
            {
                propertySchema["properties"] = new JsonObject();

                var properties = type.GetProperties();
                foreach (var childProperty in properties)
                {
                    var childSchema = GetPropertySchema(childProperty);
                    if (childSchema != null)
                    {
                        ((JsonObject)propertySchema["properties"]).Add(childProperty.Name, childSchema);
                    }
                }
            }

            return propertySchema;
        }
    }
}

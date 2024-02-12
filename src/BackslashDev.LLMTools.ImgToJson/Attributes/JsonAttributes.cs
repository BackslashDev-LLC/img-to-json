namespace BackslashDev.LLMTools.ImgToJson.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public JsonDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonEnumAttribute : Attribute
    {
        public string[] Enum { get; }

        public JsonEnumAttribute(params string[] enumValues)
        {
            Enum = enumValues;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SchemaIgnoreAttribute : Attribute
    {
        public SchemaIgnoreAttribute() { }
    }
}

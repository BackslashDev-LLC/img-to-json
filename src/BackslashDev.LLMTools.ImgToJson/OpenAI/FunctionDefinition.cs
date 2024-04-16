using Newtonsoft.Json.Linq;

namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    public class FunctionDefinition
    {
        public string? Description { get; set; }
        public string Name { get; set; } = string.Empty;
        public JObject? Parameters { get; set; }
    }
}

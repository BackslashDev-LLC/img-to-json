using Newtonsoft.Json;

namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    internal class Request
    {
        public List<Message> Messages { get; set; } = new List<Message>();
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }
        public string Model { get; set; } = string.Empty;
        public List<Tool>? Tools { get; set; }
        [JsonProperty("tool_choice")]
        public ToolChoice? ToolChoice { get; set; }
    }
}

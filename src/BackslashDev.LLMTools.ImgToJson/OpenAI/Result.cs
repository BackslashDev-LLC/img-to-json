using Newtonsoft.Json;

namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    internal class Result
    {
        public string Id { get; set; } = string.Empty;
        public Usage Usage { get; set; } = new Usage();
        public List<Choice> Choices { get; set; } = new List<Choice>();
    }

    internal class Usage
    {
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }
    }

    internal class Choice
    {
        public JsonMessage? Message { get; set; }
    }

    internal class JsonMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}

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
        public ResultMessage? Message { get; set; }
    }

    internal class ResultMessage
    {
        public string Role { get; set; } = string.Empty;
        public string? Content { get; set; }
        [JsonProperty("tool_calls")]
        public List<ToolCall>? ToolCalls { get; set; }
    }

    internal class ToolCall
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "function";
        [JsonProperty("function")]
        public FunctionCall? FunctionCall { get; set; }
    }

    internal class FunctionCall
    {
        public string Name { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
    } 
}

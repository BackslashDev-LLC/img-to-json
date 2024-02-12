namespace BackslashDev.LLMTools.Interfaces.Models
{
    public class VisionResult<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? ResultObject { get; set; }
        public Usage Usage { get; set; } = new Usage();
    }

    public class Usage
    {
        public int PromptTokens { get;}
        public int CompletionTokens { get; }
        public int TotalTokens { get; }

        public Usage() { }

        public Usage(int prompt, int completion, int total)
        {
            PromptTokens = prompt;
            CompletionTokens = completion;
            TotalTokens = total;
        }
    }
}

namespace BackslashDev.LLMTools.Interfaces.Config
{
    public class OpenAIImageOptions
    {
        public const string Position = "OpenAIImageConfig";

        public string ApiKey { get; set; } = string.Empty;
        public int RetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 180;
        public string VisionModel { get; set; } = "gpt-4-vision-preview";
        public int MaxTokens { get; set; } = 3000;
    }
}

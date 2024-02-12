namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    internal class Message
    {
        public string Role { get; set; } = string.Empty;
        public List<MessageContent> Content { get; set; } = new List<MessageContent>();
    }
}

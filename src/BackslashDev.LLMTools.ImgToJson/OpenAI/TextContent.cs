namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    internal class TextContent : MessageContent
    {
        public string Text { get; }

        public TextContent(string text) 
        {
            Type = "text";
            Text = text;
        }
    }
}

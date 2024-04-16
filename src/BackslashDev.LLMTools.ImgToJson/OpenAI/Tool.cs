namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    public class Tool
    {
        public string Type { get; set; } = "function";
        public FunctionDefinition? Function { get; set; }
    }
}

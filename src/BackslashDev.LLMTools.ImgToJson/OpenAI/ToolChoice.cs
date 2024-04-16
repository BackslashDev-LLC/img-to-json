namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    public class ToolChoice
    {
        public string Type { get; set; } = "function";
        public FunctionChoice? Function { get; set; }

        public static ToolChoice ChooseFunction(string name)
        {
            return new ToolChoice
            {
                Function = new FunctionChoice
                {
                    Name = name
                }
            };
        }

    }

    public class FunctionChoice
    {
        public string Name { get; set; } = string.Empty;
    }
}

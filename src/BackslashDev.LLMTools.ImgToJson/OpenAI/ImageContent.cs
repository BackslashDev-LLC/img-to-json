using BackslashDev.LLMTools.Interfaces.Enum;
using Newtonsoft.Json;

namespace BackslashDev.LLMTools.ImgToJson.OpenAI
{
    internal class ImageContent : MessageContent
    {
        [JsonProperty("image_url")]
        public Url ImageUrl { get; }
        public ImageContent(string url, VisionQuality quality)
        {
            Type = "image_url";
            ImageUrl = new Url(url, quality);
        }
    }

    internal class Url
    {
        [JsonProperty("url")]
        public string UrlString { get; }
        public string Detail { get; }

        public Url(string url, VisionQuality quality)
        {
            UrlString = url;
            switch (quality)
            {
                case VisionQuality.Auto:
                    Detail = "auto";
                    break;
                case VisionQuality.High:
                    Detail = "high";
                    break;
                case VisionQuality.Low:
                    Detail = "low";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(quality));
            }
        }
    }
}

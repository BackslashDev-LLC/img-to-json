using BackslashDev.LLMTools.Interfaces.Enum;
using BackslashDev.LLMTools.Interfaces.Models;

namespace BackslashDev.LLMTools.Interfaces
{
    public interface IVisionService
    {
        public Task<VisionResult<T>> ImageToJson<T>(string imageUrl, VisionQuality quality = VisionQuality.Auto);
        public Task<VisionResult<T>> ImageToJson<T>(byte[] imageData, ImageFormat format, VisionQuality quality = VisionQuality.Auto);
    }
}
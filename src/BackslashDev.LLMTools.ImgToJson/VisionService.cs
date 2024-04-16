using BackslashDev.LLMTools.ImgToJson.Json;
using BackslashDev.LLMTools.ImgToJson.OpenAI;
using BackslashDev.LLMTools.Interfaces;
using BackslashDev.LLMTools.Interfaces.Config;
using BackslashDev.LLMTools.Interfaces.Enum;
using BackslashDev.LLMTools.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace BackslashDev.LLMTools.ImgToJson
{
    public class VisionService : IVisionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VisionService> _logger;

        private readonly OpenAIImageOptions _options;

        public VisionService(IHttpClientFactory clientFactory, ILogger<VisionService> logger, IOptions<OpenAIImageOptions> options)
        {
            _httpClientFactory = clientFactory;
            _logger = logger;
            _options = options.Value;
        }

        public Task<VisionResult<T>> ImageToJson<T>(string imageUrl, VisionQuality quality = VisionQuality.Auto)
        {
            return IssueRequest<T>(imageUrl, quality);
        }

        public Task<VisionResult<T>> ImageToJson<T>(byte[] imageData, ImageFormat format, VisionQuality quality = VisionQuality.Auto)
        {
            return IssueRequest<T>(ToBase64(imageData, format), quality);
        }

        private string ToBase64(byte[] imageData, ImageFormat format)
        {
            _logger.LogTrace("Converting image to base64");

            var base64Image = Convert.ToBase64String(imageData);
            var mimeType = format switch
            {
                ImageFormat.JPEG => "image/jpeg",
                ImageFormat.PNG => "image/png",
                ImageFormat.WEBP => "image/webp",
                ImageFormat.GIF => "image/gif",
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };

            return $"data:{mimeType};base64,{base64Image}";
        }

        private async Task<VisionResult<T>> IssueRequest<T>(string imageUrl, VisionQuality quality)
        {
            var schema = SchemaGenerator.GenerateJsonSchema<T>();

            _logger.LogTrace("Generated schema {schema}", schema);

            var js = schema.ToJsonString();

            var extractData = new Tool
            {
                Function = new FunctionDefinition
                {
                    Name = "extract_data",
                    Description = "Extracts data from a provided image into the expected JSON schema",
                    Parameters = JsonConvert.DeserializeObject<JObject>(js)
                }
            };

            var image = new Message
            {
                Role = "user",
                Content = new List<MessageContent> { new ImageContent(imageUrl, quality) }
            };

            var request = new Request
            {
                MaxTokens = _options.MaxTokens,
                Messages = new List<Message> { image },
                Model = _options.VisionModel,
                Tools = new List<Tool> { extractData },
                ToolChoice = ToolChoice.ChooseFunction(extractData.Function.Name)
            };

            var client = _httpClientFactory.CreateClient("OpenAIImageClient");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonPayload = JsonConvert.SerializeObject(request, settings);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogTrace("Issuing request to OpenAI");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content).ConfigureAwait(false);
            var strResult = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new VisionResult<T>
                {
                    Success = false,
                    ErrorMessage = strResult
                };
            }

            var apiResult = JsonConvert.DeserializeObject<Result>(strResult);

            if (apiResult == null || !apiResult.Choices.Any())
            {
                return new VisionResult<T>
                {
                    Success = false,
                    ErrorMessage = "Unexpected result from API " + strResult
                };
            }

            var choice = apiResult.Choices.First();

            if (choice.Message == null || choice.Message.ToolCalls == null)
            {
                return new VisionResult<T>
                {
                    Success = false,
                    ErrorMessage = "Unexpected result from API - No tool call present. " + strResult
                };
            }

            var tool = choice.Message.ToolCalls.FirstOrDefault(c => c.FunctionCall?.Name == extractData.Function.Name);

            if (tool == null || tool.FunctionCall == null)
            {
                return new VisionResult<T>
                {
                    Success = false,
                    ErrorMessage = "Unexpected result from API - No function call present. " + strResult
                };
            }

            var jsonResult = tool.FunctionCall.Arguments;

            try
            {
                var obj = JsonConvert.DeserializeObject<T>(jsonResult);

                if (obj != null)
                {
                    return new VisionResult<T>
                    {
                        Success = true,
                        ResultObject = obj,
                        Usage = new Interfaces.Models.Usage(apiResult.Usage.PromptTokens, apiResult.Usage.CompletionTokens, apiResult.Usage.TotalTokens)
                    };
                }
            }
            catch (Exception)
            {
            }

            return new VisionResult<T>
            {
                Success = false,
                ErrorMessage = "Unable to serialize response " + jsonResult
            };
        }
    }
}
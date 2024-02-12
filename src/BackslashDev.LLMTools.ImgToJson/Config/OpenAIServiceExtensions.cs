using BackslashDev.LLMTools.Interfaces;
using BackslashDev.LLMTools.Interfaces.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Extensions.Http;

namespace BackslashDev.LLMTools.ImgToJson.Config
{
    public static class OpenAIServiceExtensions
    {
        public static IServiceCollection AddImgToJson(this IServiceCollection services, IConfiguration configuration)
        {            
            var configSection = configuration.GetRequiredSection(OpenAIImageOptions.Position);
            var config = configSection.Get<OpenAIImageOptions>();

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(config.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient("OpenAIImageClient", cfg =>
            {
                cfg.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            }).AddPolicyHandler(retryPolicy);

            services.Configure<OpenAIImageOptions>(configSection);

            services.AddScoped<IVisionService, VisionService>();

            return services;
        }
    }
}

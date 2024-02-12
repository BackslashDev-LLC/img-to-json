using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using BackslashDev.LLMTools.ImgToJson.Config;
using Microsoft.Extensions.DependencyInjection;
using Demo.Cli;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddImgToJson(builder.Configuration);

builder.Services.AddScoped<IProcessor, Processor>();

var host = builder.Build();

RunProgram(host.Services);

await host.RunAsync();

static async void RunProgram(IServiceProvider hostProvider)
{
    var app = hostProvider.GetRequiredService<IProcessor>();

    await app.ProcessFiles().ConfigureAwait(false);

    Console.ReadKey();
}

using BackslashDev.LLMTools.Interfaces;
using BackslashDev.LLMTools.Interfaces.Enum;
using Demo.Cli.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Cli
{
    public interface IProcessor
    {
        Task ProcessFiles();
    }
    public class Processor : IProcessor
    {
        private readonly IVisionService _visionService;

        public Processor(IVisionService visionService)
        {
            _visionService = visionService;
        }

        public async Task ProcessFiles()
        {
            Console.WriteLine("Starting processing files");

            foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image_samples")))
            {
                Console.WriteLine("Processing file " + file);
                var fileBytes = await File.ReadAllBytesAsync(file);

                var resumeResult = await _visionService.ImageToJson<Resume>(fileBytes, ImageFormat.JPEG).ConfigureAwait(false);

                if (!resumeResult.Success || resumeResult.ResultObject == null)
                {
                    Console.WriteLine(resumeResult.ErrorMessage);
                    continue;
                }

                var resume = resumeResult.ResultObject;

                Console.WriteLine($"Processing took {resumeResult.Usage.TotalTokens} tokens.");

                Console.WriteLine($"Applicant {resume.ApplicantName} can be reached by phone at {resume.Phone ?? "Unknown"} or {resume.Email ?? "Unknown"}");
                Console.WriteLine("Highest level of education: " + resume.HighestLevelOfEducation);

                foreach(var workExperience in resume.Experience)
                {
                    Console.WriteLine($"    Worked at {workExperience.Job} from {workExperience.StartDate} to {workExperience.EndDate}. Relevance: {workExperience.Relevancy}%");
                    foreach(var desc in workExperience.Description)
                    {
                        Console.WriteLine("        " + desc);
                    }
                }
            }
        }
    }
}

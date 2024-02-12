using BackslashDev.LLMTools.ImgToJson.Attributes;

namespace Demo.Cli.Models
{
    public class Resume
    {
        public string ApplicantName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        [JsonEnum("high school", "some college", "bachelors", "masters", "doctoral")]
        public string HighestLevelOfEducation { get; set; } = string.Empty;
        public List<WorkExperience> Experience { get; set; } = new List<WorkExperience>();
    }

    public class WorkExperience
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        [JsonDescription("The job title and employer formatted as '{jobTitle} at {employer}'")]
        public string Job { get; set; }= string.Empty;
        public List<string> Description { get; set; } = new List<string>();
        [JsonDescription("How relevant is this job to my open posting for an 'Administrative Assistant' on a scale of 0 (not at all) to 100 (completely)")]
        public int Relevancy { get; set; }
    }
}

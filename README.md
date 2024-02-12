# Image to JSON

A simple C# .NET library that allows a developer to extract structured data from an image input using OpenAI's GPT4 Vision Model.

Currently the OpenAI GPT4 Vision Service does not make use of tools (like function calling), and you cannot specify a JSON only response. This makes it difficult to receive structured data from a Vision query. This library seeks to resolve that issue.

## Pre-Requisites

To make use of this repository you will need to have your own OpenAI API Key. To obtain a key please visit the [OpenAI Website](https://openai.com/).

For full information about OpenAI's Vision Service, review [their documentation](https://platform.openai.com/docs/guides/vision)

## Demo

This repository contains a Command Line demonstration of the library for interested parties. Simply clone this repository and open the solution file at the root. The demo parses 3 resume images into a structured result. More detail about the demo is available in [the project itself](https://github.com/backslashdev-llc/img-to-json/demo).

## Usage

### Installation

The `ImgToJson` package is distributed via NuGet for easy integration into your project. You can find the package in the [NuGet Repository](https://www.nuget.org/packages/BackslashDev.LLMTools.ImgToJson/)

To install in your project use:

```cli
dotnet add package BackslashDev.LLMTools.ImgToJson
```

### Configuration

You'll need to add the following configuration to your `appsettings.json`.

```json
"OpenAIImageConfig": {
    "ApiKey": "<insert-your-key>",
    "RetryCount": 3,
    "TimeoutSeconds": 180,
    "VisionModel": "gpt-4-vision-preview",
    "MaxTokens": 4000
  }
```

- **ApiKey:** Your API Key From OpenAI. Keep this value a secret
- **RetryCount:** The library implements an exponential back-off for retries. This is the number of times you want a request resulting in a transient (tempory) failure to be retried.
- **TimeoutSeconds:** The maximum number of seconds to wait for a response from OpenAI
- **VisionModel:** The name of the model you wish to use
- **MaxTokens:** The maximum number of response tokens requested from OpenAI. JSON results consume a lot of tokens, so you'll want to keep this value high. Currently the maximum value is `4096`.

### Register Dependencies

If you are using standard .NET Dependency Injection, you can register the `VisionService` dependency with:

```C#
using BackslashDev.LLMTools.ImgToJson.Config;

// ...other startup code...

services.AddImgToJson(Configuration);
```

### Configuring Your Response Object

In most cases, the library will handle instructing OpenAI on how to convert its response into your desired format automatically. There may be additional use cases where you'll want to tailor the specific output. For these purposes, the library exposes several property attributes:

`[JsonDescription]`

Use this attribute to explain to OpenAI how a particular property should be interpreted.

`[JsonEnum]`

Use this attribute to limit the values that OpenAI can choose from for a particular property.

`[SchemaIgnore]`

Use this attribute to have OpenAI ignore this property.

#### Example

```C#
using BackslashDev.LLMTools.ImgToJson.Attributes;

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
```

### Calling the Service

The `IVisionService` interface exposes two methods:

1. Extract JSON from an image hosted on a publicly accessible URL
1. Extract JSON from a local image

In the second case, the image will be converted to base64 prior to being sent to OpenAI. Generally, performance is slightly better using the first method.

_Important Note_

Currently, OpenAI only supports files in `JPEG`, `PNG`, `WEBP` or `GIF` formats.

#### Inject the Service

```C#
private readonly IVisionService _visionService;

public Processor(IVisionService visionService)
{
    _visionService = visionService;
}
```

#### Call one of the extraction methods, passing the annotated object you want returned

```C#
var resumeResult = await _visionService.ImageToJson<Resume>(fileBytes, ImageFormat.JPEG);
```

_Note -The ImageFormat is only needed when using the base64 method._

#### Handle the Response

The service will respond with an object of type `VisionResult<T>`. You can check whether the request was `Successful`. If not, you can retrieve the `ErrorMessage`. If it was, the populated object is available at `ResultObject`.

## License

This repository and the associated NuGet packages are licensed under the terms of the GPL V3 license. Full details can be found in [the license file](https://github.com/backslashdev-llc/img-to-json/LICENSE.md).

## Contribution

Contributions from the community are welcome. Please open a PR explaining your change. If you have any problems with the library or suggestions for improvement please open an issue. It is expected that the need for this library will likely be removed in some future iteration of OpenAI's API development, but for now, I hope this helps!

## About BackslashDev

BackslashDev is an Upscale Software Development boutique based out of Phoenix, AZ. We specialize in leveraging leading edge technologies in underserved sectors like Healthcare and GovTech. Learn more about us on [our website](https://www.backslashdev.com?utm_source=imgtojson)

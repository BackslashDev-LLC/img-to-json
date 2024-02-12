# Image to JSON Demo

This is a demonstration of the `img-to-json` library. This project contains a folder with 3 JPEG images of sample resumes (these are not real people).

Be sure that the `Demo.Cli` project is selected as the start up project, and then click debug. The project will iterate over each resume image and utilize the `img-to-json` library to extract data from each.

## Configuration

To run the demo you will need to create an `appsettings.local.json` file in the `Demo.Cli` project and populate it with your OpenAI key like this:

```json
{
  "OpenAIImageConfig": {
    "ApiKey": "<your-open-ai-key>"
  }
}
```

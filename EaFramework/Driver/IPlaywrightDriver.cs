using Microsoft.Playwright;

namespace EaFramework.Driver;

public interface IPlaywrightDriver
{
    Task<IPage> Page { get; }
    Task<IBrowser> Browser { get; }
    Task<IBrowserContext> BrowserContext { get; }

    /// <summary>
    /// Captures a screenshot and saves it to a file path.
    /// </summary>
    /// <param name="fileName">The name of the file where the screenshot will be saved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the full file path where the screenshot was saved.</returns>
    Task<string> ScreenshotAsPathAsync(string fileName);
}
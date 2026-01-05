using System;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Reporter;
using EaFramework.Config;
using EaFramework.Driver;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace EaSpecflowTests.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly TestSettings _testSettings;
        private readonly IPlaywrightDriver _playwrightDriver;
        private readonly FeatureContext _featureContext;

        private readonly Task<IPage> _page;
        private static ExtentReports _extentReports;
        private static ScenarioContext _scenarioContext;
        private static ExtentTest _scenario;

        public Hooks(IPlaywrightDriver playwrightDriver, TestSettings testSettings, FeatureContext featureContext, ScenarioContext scenarioContext)
        
        {
            _playwrightDriver = playwrightDriver;
            _testSettings = testSettings;
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _page = playwrightDriver.Page;
        }
        
        [BeforeTestRun]
        public static void InitializeExtentReports()
        {
            _extentReports = new ExtentReports();
            _extentReports.AddSystemInfo("os","Windows");
            _extentReports.AddSystemInfo("browser","Chrome");
            _extentReports.AddSystemInfo("browserVersion","98.0");
            var spark = new ExtentSparkReporter(filePath: "ExtentReports.html");
            _extentReports.AttachReporter(spark);
            var reportDir = Path.Combine(
                     AppContext.BaseDirectory,
                        "..", "..", "..", "..",
                        "EaSpecflowTests",
                        "TestResults"
                );

            Directory.CreateDirectory(reportDir);

            var reportPath = Path.Combine(reportDir, "ExtentReport.html");

            var htmlReporter = new ExtentSparkReporter(reportPath);

            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(htmlReporter);
        }
        
        [AfterStep]
      public async Task AfterStep()
    {
         var fileName = $"{_featureContext.FeatureInfo.Title.Trim()}_" + $"{Regex.Replace(_scenarioContext.ScenarioInfo.Title, @"\s", "")}";
        if (_scenarioContext.TestError == null)
    {
        // 1. Await the Page task to get the IPage object
        var page = await _playwrightDriver.Page; 

        // 2. Now you can call ScreenshotAsync on the actual page object
        var screenshotBytes = await page.ScreenshotAsync();

        // 3. Convert to Base64 to avoid broken image links in the report
        string base64String = Convert.ToBase64String(screenshotBytes);

        var media = MediaEntityBuilder
            .CreateScreenCaptureFromBase64String(base64String)
            .Build();

        var stepText = _scenarioContext.StepContext.StepInfo.Text;

        // 4. Log to Extent Report
        switch (_scenarioContext.StepContext.StepInfo.StepDefinitionType)
        {
            case StepDefinitionType.Given:
                _scenario.CreateNode<Given>(stepText).Pass(stepText, media);
                break;
            case StepDefinitionType.When:
                _scenario.CreateNode<When>(stepText).Pass(stepText, media);
                break;
            case StepDefinitionType.Then:
                _scenario.CreateNode<Then>(stepText).Pass(stepText, media);
                break;
        }
    }
            else
            {
                //failing steps
                switch(_scenarioContext.StepContext.StepInfo.StepDefinitionType)
                {
                    case StepDefinitionType.Given:
                        _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.Message, media: new ScreenCapture
                        {
                            Title = "Error Screenshot",
                            Path = await _playwrightDriver.ScreenshotAsPathAsync(fileName)
                        });
                        break;
                    case StepDefinitionType.When:
                        _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.Message, media: new ScreenCapture
                        {
                            Title = "Error Screenshot",
                            Path = await _playwrightDriver.ScreenshotAsPathAsync(fileName)
                        });
                        break;
                    case StepDefinitionType.Then:
                        _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.Message, media: new ScreenCapture
                        {
                            Title = "Error Screenshot",
                            Path = await _playwrightDriver.ScreenshotAsPathAsync(fileName)
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await (await _page).GotoAsync(_testSettings.ApplicationUrl);

            //Feature Title
            var feature = _extentReports.CreateTest<Feature>(_featureContext.FeatureInfo.Title);
            //Scenario Title
            _scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

        }

        [AfterTestRun]
        public static void TearDownExtentReports() => _extentReports.Flush();
        
    }
}

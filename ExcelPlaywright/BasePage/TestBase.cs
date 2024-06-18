using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using ExcelPlaywright.config;
using System.Text.Json;

namespace ExcelPlaywright
{
    [TestFixture]
    public class TestBase
    {
        public static IBrowser _browser;
        public static IPage _page;
        protected static ExtentReports _extent;
        protected static ExtentTest _test;
        public static string USERNAME;
        public static string PASSWORD;
        public static string JuniperProgramName;
        public static Dictionary<string, string> testDataFromInput;
        public static string systemEnvironment;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Extent report initialization
            if (_extent == null)
            {
                string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                Directory.CreateDirectory(reportPath);
                string reportFile = Path.Combine(reportPath, "ExtentReport.html");

                var sparkReporter = new ExtentSparkReporter(reportFile);
                _extent = new ExtentReports();
                _extent.AttachReporter(sparkReporter);
                _extent.AddSystemInfo("User", "test");
                _extent.AddSystemInfo("Host Name", "LocalHost");
            }

            // Browser setup
            var configSettings = new configSettings();
            string browserType = configSettings.Browser;

            var playwright = await Playwright.CreateAsync();
            BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[] { "--disable-web-security", "--disable-features=IsolateOrigins,site-per-process", "--allow-running-insecure-content" }
               // Args = new[] { "--disable-web-security", "--disable-features=IsolateOrigins,site-per-process" }
            };

            switch (browserType.ToLower())
            {
                case "chrome":
                    _browser = await playwright.Chromium.LaunchAsync(options);
                    break;
                case "firefox":
                    _browser = await playwright.Firefox.LaunchAsync(options);
                    break;
                case "edge":
                    _browser = await playwright.Chromium.LaunchAsync(options);
                    break;
                default:
                    throw new ArgumentException("Please add a valid browser in Config Settings");
            }

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();

            // System Environment setup
            var testFullName = TestContext.CurrentContext.Test.FullName.ToLower();
            systemEnvironment = GetAppSettingJsonData("dynamicevnts").ToLower();

            if (systemEnvironment.Equals("#{enviorment}#"))
            {
                systemEnvironment = "demo";
            }

            if (testFullName.Contains("polycpq") || testFullName.Contains("poly_cpq"))
            {
                //  _page.GotoAsync("your_polycpq_url").Wait();
            }
            else if (testFullName.Contains("cisco_portal") && testFullName.Contains("userstories"))
            {
                //_page.GotoAsync("").Wait();
            }
            else if (testFullName.Contains("cisco_portal"))
            {
                //_page.GotoAsync("your_cisco_portal_url").Wait();
            }
            else
            {
                if (systemEnvironment.Equals("demo"))
                {
                    _page.GotoAsync("https://demo.citplatform.com/login.aspx?ReturnUrl=%2fProtected%2fCM%2fWelcome.aspx").Wait();
                    USERNAME = configSettings.DemoAdminUsername;
                    PASSWORD = configSettings.DemoAdminPassword;
                }
                else if (systemEnvironment.Equals("sit"))
                {
                    //_page.GotoAsync("your_sit_url").Wait();
                    //USERNAME = configSettings.SITAdminUsename;
                    //PASSWORD = configSettings.SITAdminPassword;
                }
                else
                {
                    //systemEnvironment = "sit";
                    //_page.GotoAsync("your_sit_url").Wait();
                    //USERNAME = configSettings.SITAdminUsename;
                    //PASSWORD = configSettings.SITAdminPassword;
                }
                await _page.SetViewportSizeAsync(1440, 900);
            }
        }

        [SetUp]
        public async Task BeforeTest()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            _test = _extent.CreateTest(testName);

            // Retrieve and assign categories
            var categories = TestContext.CurrentContext.Test.Properties["Category"];
            if (categories != null)
            {
                foreach (string category in categories)
                {
                    _test.AssignCategory(category);
                }
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            try
            {
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in TearDown: " + ex.Message);
            }
            finally
            {
                if (_extent != null)
                {
                    _extent.Flush();
                    Console.WriteLine("Extent report flushed");

                    // Check if the report file exists
                    string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "ExtentReport.html");
                    if (File.Exists(reportPath))
                    {
                        Console.WriteLine("Report generated successfully: " + reportPath);
                    }
                    else
                    {
                        Console.WriteLine("Report not found: " + reportPath);
                    }
                }
            }
        }

        [TearDown]
        public async Task AfterTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMsg = TestContext.CurrentContext.Result.Message;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace) ? "" : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
            Status logStatus;

            switch (status)
            {
                case TestStatus.Failed:
                    logStatus = Status.Fail;
                    _test.Log(logStatus, $"Test ended with {logStatus} - {errorMsg}");
                    _test.Log(logStatus, $"Stacktrace: {stacktrace}");
                    await AddScreenshot("FailedTestScreenshot");
                    break;
                case TestStatus.Inconclusive:
                    logStatus = Status.Warning;
                    _test.Log(logStatus, $"Test ended with {logStatus}");
                    break;
                case TestStatus.Skipped:
                    logStatus = Status.Skip;
                    _test.Log(logStatus, $"Test ended with {logStatus}");
                    break;
                default:
                    logStatus = Status.Pass;
                    _test.Log(logStatus, "Test ended with success");
                    await AddScreenshot("SuccessTestScreenshot");
                    break;
            }
        }

        public async Task AddScreenshot(string elementName = null)
        {
            DateTime time = DateTime.Now;
            string fileName = elementName == null
                ? "Screenshot_" + time.ToString("h_mm_ss") + ".png"
                : "SS_" + elementName + "_" + time.ToString("h_mm_ss") + ".png";

            string screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Screenshots");
            Directory.CreateDirectory(screenshotPath); // Ensure the directory exists
            string finalPath = Path.Combine(screenshotPath, fileName);

            // Capture the screenshot
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = finalPath });

            // Use relative path for the screenshot in the report
            string relativePath = Path.Combine("Screenshots", fileName);

            _test.AddScreenCaptureFromPath(relativePath);

            TestContext.AddTestAttachment(finalPath);
        }

        private string GetAppSettingJsonData(string environmentKey)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
            string configFilePath = Path.Combine(projectDirectory, "config", "QASettings.json");

            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException($"The config file was not found: {configFilePath}");
            }

            var json = File.ReadAllText(configFilePath);
            var config = JsonDocument.Parse(json);

            return config.RootElement.GetProperty("Envt").GetProperty(environmentKey).GetString();
        }
    }
}





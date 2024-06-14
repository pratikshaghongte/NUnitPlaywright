using ExcelDataReader;
using Microsoft.Playwright;
using System.Reflection;
using System.Threading.Tasks;

namespace ExcelPlaywright.Utils
{
    public class TestUtils : TestBase
    {
        //private readonly IPage _page;
        private IPage _originalPage;
        //private readonly IBrowser _browser;
       
        public TestUtils(IPage page, IBrowser browser)
        {
            _page = page;
            //_browser = browser ?? throw new ArgumentNullException(nameof(browser));
        }

        //Click
        public async Task Click(string selector) => await _page.ClickAsync(selector);

        public async Task Click(string selector, float positionX, float positionY) =>
        await _page.Locator(selector).ClickAsync(new() { Position = new Position { X = positionX, Y = positionY } });

        //Double click
        public async Task DoubleClick(string selector) => await _page.DblClickAsync(selector);

        //Fill input fields
        public async Task FillField(string selector, string? value) => await _page.FillAsync(selector, value);

        // Switch to a frame by its selector
        public async Task<IFrame> SwitchFrameAsync(string frameSelector)
        {
            var frameElementHandle = await _page.QuerySelectorAsync(frameSelector);
            if (frameElementHandle != null)
            {
                var frame = await frameElementHandle.ContentFrameAsync();
                if (frame != null)
                {
                    return frame;
                }
                else
                {
                    throw new Exception("Frame not found.");
                }
            }
            else
            {
                throw new Exception("Frame element not found.");
            }
        }

        //public async Task<IFrame> SwitchFrameAsync(string frameSelector)
        //{
        //    var elementHandle = await _page.QuerySelectorAsync(frameSelector);
        //    if (elementHandle == null)
        //    {
        //        throw new TimeoutException($"Frame with selector '{frameSelector}' not found.");
        //    }
        //    return await elementHandle.ContentFrameAsync();
        //}
        //public async Task<IFrame> SwitchFrameAsync(string frameSelector)
        //{
        //    Console.WriteLine($"[DEBUG] Attempting to switch to frame with selector: {frameSelector}");

        //    // Wait for the iframe to be present
        //    await WaitForSelectorStateAsync(_page, frameSelector, ElementState.Visible);

        //    // Query the iframe element
        //    var elementHandle = await _page.QuerySelectorAsync(frameSelector);

        //    if (elementHandle == null)
        //    {
        //        Console.WriteLine($"[ERROR] Frame with selector '{frameSelector}' not found.");
        //        throw new TimeoutException($"Frame with selector '{frameSelector}' not found.");
        //    }

        //    var frame = await elementHandle.ContentFrameAsync();

        //    if (frame == null)
        //    {
        //        Console.WriteLine($"[ERROR] Unable to switch to frame '{frameSelector}'.");
        //        throw new TimeoutException($"Unable to switch to frame '{frameSelector}'.");
        //    }

        //    Console.WriteLine($"[DEBUG] Successfully switched to frame with selector: {frameSelector}");
        //    return frame;
        //}

        public async Task SwitchToDefaultFrameAsync()
        {
            await _page.SetContentAsync(await _page.ContentAsync());
        }


        //Toggle Checkbox
        public async Task ToggleCheckBox(string selector, bool check = true)
        {
            await _page.Locator(selector).SetCheckedAsync(check);
        }

        //ALert Box
        public async Task HandleAlertBoxAsync(string acceptOrReject)
        {
            _page.Dialog += async (_, dialog) =>
            {
                if (acceptOrReject.ToLower() == "accept")
                {
                    await dialog.AcceptAsync();
                }
                else if (acceptOrReject.ToLower() == "reject")
                {
                    await dialog.DismissAsync();
                }
            };
        }

        public async Task ScrollToElementAsync(string selector)
        {
            var element = await _page.QuerySelectorAsync(selector);
            if (element != null)
            {
                await element.ScrollIntoViewIfNeededAsync();
            }
            else
            {
                throw new Exception($"Element with selector {selector} not found.");
            }
        }


        //select by name from dropdown
        public async Task SelectByNameAsync(string selector, string optionText)
        {
            var element = await _page.QuerySelectorAsync(selector);
            if (element != null)
            {
                var option = await element.QuerySelectorAsync($"option:has-text(\"{optionText}\")");
                if (option != null)
                {
                    var value = await option.GetAttributeAsync("value");
                    await element.SelectOptionAsync(new[] { value });
                }
                else
                {
                    throw new Exception($"Option with text '{optionText}' not found.");
                }
            }
            else
            {
                throw new Exception($"Element with selector {selector} not found.");
            }
        }
        public async Task NavigateURL(string url)
        {
            await _page.GotoAsync(url);
        }
        public static string GetCurrentTime()
        {
            return Regex.Replace(DateTime.Now.ToString(), @"[^0-9a-zA-Z]+", "");
        }
        public static async Task MouseOver(IPage page, string selector)
        {
            await page.HoverAsync(selector);
        }
        public static async Task WaitForMovement(IPage page)
        {
            // Example: Wait for a specific element to be visible
            // Replace 'selector' with the actual CSS selector of the element you're waiting for
            await page.WaitForSelectorAsync("selector", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible
            });
        }

        public static async Task ClickOnButton(IPage page, string selector)
        {
            var button = await page.QuerySelectorAsync(selector);
            if (button != null)
            {
                await button.ClickAsync();
            }
            else
            {
                throw new Exception($"Button with selector '{selector}' not found.");
            }
        }
        public static async Task RightClickAction(IPage page, string selector)
        {
            var element = await page.QuerySelectorAsync(selector);
            if (element != null)
            {
                await element.ClickAsync(new ElementHandleClickOptions { Button = MouseButton.Right });
            }
            else
            {
                throw new Exception($"Element with selector '{selector}' not found.");
            }
        }

        private static string GetProjectPath()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var actualPath = Path.GetDirectoryName(path);
            var projectPath = Path.Combine(actualPath, "Text1.txt");
            return projectPath;
        }
        public static async Task StoreGlobalRecord(string data)
        {
            var projectPath = GetProjectPath();
            try
            {
                await File.WriteAllTextAsync(projectPath, data);
            }
            catch (Exception e)
            {
                TestContext.WriteLine(e);
            }
        }

        public static void DeleteGlobalRecord()
        {
            var projectPath = GetProjectPath();
            try
            {
                if (File.Exists(projectPath))
                {
                    File.Delete(projectPath);
                }
            }
            catch (Exception e)
            {
                TestContext.WriteLine(e);
            }
        }

        public static async Task<string> GetGlobalRecord()
        {
            var projectPath = GetProjectPath();
            try
            {
                if (File.Exists(projectPath))
                {
                    var data = await File.ReadAllTextAsync(projectPath);
                    TestContext.Progress.WriteLine(data);
                    return data;
                }
            }
            catch (Exception e)
            {
                TestContext.Progress.WriteLine(e);
            }
            return null;
        }
     

        public async Task WaitForSelectorStateAsync(IPage page, string selector, ElementState state, int timeout = 30000)
        {
            var waitOptions = new PageWaitForSelectorOptions
            {
                Timeout = timeout,
                State = state switch
                {
                    ElementState.Visible => WaitForSelectorState.Visible,
                    ElementState.Hidden => WaitForSelectorState.Hidden,
                    ElementState.Enabled => WaitForSelectorState.Visible,
                    ElementState.Disabled => WaitForSelectorState.Attached,
                    _ => WaitForSelectorState.Attached
                }
            };

            var element = await page.WaitForSelectorAsync(selector, waitOptions);
            if (element == null)
            {
                throw new TimeoutException($"Element '{selector}' not found within the specified timeout of {timeout} milliseconds.");
            }
        }
        

        public static void LoadExcelData(string fileName, string sheetName)
        {
            // Define the path to the Excel file
            var workingDirectory = Environment.CurrentDirectory;
            var projectPath = workingDirectory.Substring(0, workingDirectory.IndexOf("bin"));
            var filePath = Path.Combine(projectPath, "TestData", fileName);

            // Validate the file extension
            if (!filePath.EndsWith(".xlsx"))
            {
                throw new ArgumentException("Input data file must be in .xlsx format");
            }

            // Open the Excel file for reading
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Read the Excel file into a DataSet
            using var excelReader = ExcelReaderFactory.CreateReader(stream);
            var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            // Get the specified sheet from the DataSet
            var table = result.Tables;
            if (!table.Contains(sheetName))
            {
                throw new ArgumentException($"Sheet '{sheetName}' not found in the Excel file");
            }
            var resultTable = table[sheetName];

            // Load data from the sheet into the dictionary
            testDataFromInput = new Dictionary<string, string>();
            for (int row = 0; row < resultTable.Rows.Count; row++)
            {
                var key = resultTable.Rows[row][0].ToString();
                var value = resultTable.Rows[row][1].ToString();
                testDataFromInput.Add(key, value);
            }
        }
        public static string GetDataByKey(string key)
        {
            if (testDataFromInput.ContainsKey(key))
            {
                return testDataFromInput[key];
            }
            else
            {
                throw new KeyNotFoundException($"The key '{key}' was not found in the test data.");
            }
        }
        public async Task SwitchTabAsync(int tab, IPage page)
        {
            var contexts = _browser.Contexts;

            if (contexts == null || contexts.Count == 0)
            {
                throw new InvalidOperationException("No browser contexts available.");
            }

            var pages = contexts[0].Pages;

            if (pages == null || pages.Count == 0)
            {
                throw new InvalidOperationException("No pages available in the browser context.");
            }

            if (tab < 0 || tab >= pages.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(tab), "Invalid tab index.");
            }

            await pages[tab].BringToFrontAsync();
        }
        public async Task WaitForNetworkIdleAsync(IPage page, int timeout = 60000)
        {
            Console.WriteLine("[DEBUG] Waiting for network to be idle...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeout });
            Console.WriteLine("[DEBUG] Network is idle.");
        }
    }
}













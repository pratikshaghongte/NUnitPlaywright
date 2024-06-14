//using ExcelDataReader;
//using Microsoft.Playwright;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ExcelPlaywright
//{
//    public class ExcelHelper
//    {

//        private static Dictionary<string, string> testDataFromInput;

//        public static void LoadExcelData(string fileName, string sheetName)
//        {
//            // Define the path to the Excel file
//            var workingDirectory = Environment.CurrentDirectory;
//            var projectPath = workingDirectory.Substring(0, workingDirectory.IndexOf("bin"));
//            var filePath = Path.Combine(projectPath, "TestData", fileName);

//            // Validate the file extension
//            if (!filePath.EndsWith(".xlsx"))
//            {
//                throw new ArgumentException("Input data file must be in .xlsx format");
//            }

//            // Open the Excel file for reading
//            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

//            // Read the Excel file into a DataSet
//            using var excelReader = ExcelReaderFactory.CreateReader(stream);
//            var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
//            {
//                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
//                {
//                    UseHeaderRow = true
//                }
//            });

//            // Get the specified sheet from the DataSet
//            var table = result.Tables;
//            if (!table.Contains(sheetName))
//            {
//                throw new ArgumentException($"Sheet '{sheetName}' not found in the Excel file");
//            }
//            var resultTable = table[sheetName];

//            // Load data from the sheet into the dictionary
//            testDataFromInput = new Dictionary<string, string>();
//            for (int row = 0; row < resultTable.Rows.Count; row++)
//            {
//                var key = resultTable.Rows[row][0].ToString();
//                var value = resultTable.Rows[row][1].ToString();
//                testDataFromInput.Add(key, value);
//            }
//        }

//        public static string GetDataByKey(string key)
//        {
//            if (testDataFromInput.ContainsKey(key))
//            {
//                return testDataFromInput[key];
//            }
//            else
//            {
//                throw new KeyNotFoundException($"The key '{key}' was not found in the test data.");
//            }
//        }
//    }

//    public class Tests1
//    {
//        [Test]
//        public async Task MyPlaywrightTest()
//        {

//            // Load the Excel data
//            ExcelHelper.LoadExcelData("C:\\Users\\pratiksha.ghongte\\Downloads\\Login.xlsx", "Sheet1");

//            // Retrieve specific data by key
//            var username = ExcelHelper.GetDataByKey("username");
//            var password = ExcelHelper.GetDataByKey("password");

//            var playwright = await Playwright.CreateAsync();
//            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
//            var page = await browser.NewPageAsync();

//            // Use the test data in your Playwright tests
//            await page.GotoAsync("https://www.saucedemo.com/v1/");
//            await page.FillAsync("#user-name", username);
//            await page.FillAsync("#password", password);
//            await page.ClickAsync("#login-button");

//            await browser.CloseAsync();
//        }
//    }
//}


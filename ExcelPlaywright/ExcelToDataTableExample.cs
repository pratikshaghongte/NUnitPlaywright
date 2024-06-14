//using Microsoft.Playwright;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ExcelDataReader;
////using ExcelDataReader.DataSet;


//namespace ExcelPlaywright
//{
//    internal class ExcelToDataTableExample
//    {
//        private static string projectPath;
//        private static string filePath;

//        public async Task<Dictionary<string, string>> ExcelToDataTable(string fileName, string sheetName)
//        {
//            string workingDirectory = Environment.CurrentDirectory;
//            projectPath = workingDirectory.Substring(0, workingDirectory.IndexOf("bin"));
//            filePath = Path.Combine(projectPath, "TestData", fileName);

//            if (!filePath.Contains("xlsx"))
//            {
//                throw new Exception("Input data file must be in .xlsx format");
//            }

//            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
//            {
//                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//                using (var excelReader = ExcelReaderFactory.CreateReader(stream))
//                {
//                    var result = excelReader.AsDataSet(new ExcelDataSetConfiguration
//                    {
//                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration
//                        {
//                            UseHeaderRow = true
//                        }
//                    });

//                    var table = result.Tables;
//                    var resultTable = table[sheetName];

//                    var testDataFromInput = new Dictionary<string, string>();

//                    for (int row = 0; row < resultTable.Rows.Count; row++)
//                    {
//                        var key = resultTable.Rows[row][0].ToString();
//                        var value = resultTable.Rows[row][1].ToString();
//                        testDataFromInput.Add(key, value);
//                    }

//                    return testDataFromInput;
//                }
//            }
//        }

//        public async Task<string> ReadDataValueByKeyAsync(string fileName, string sheetName, string key)
//        {
//            var testData = await ExcelToDataTable(fileName, sheetName);
//            if (testData.ContainsKey(key))
//            {
//                return testData[key];
//            }
//            else
//            {
//                throw new KeyNotFoundException($"Key '{key}' not found in the data table.");
//            }
//        }

//        [Test]
//        public async Task MainAsync()
//        {
//            var playwright = await Playwright.CreateAsync();
//            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
//            var page = await browser.NewPageAsync();

//            // Example usage
//            var value = await ReadDataValueByKeyAsync("C:\\Users\\pratiksha.ghongte\\Downloads\\Login.xlsx", "Sheet1", "username");
//            Console.WriteLine($"Value: {value}");

//            await browser.CloseAsync();
//        }
//    }
//}

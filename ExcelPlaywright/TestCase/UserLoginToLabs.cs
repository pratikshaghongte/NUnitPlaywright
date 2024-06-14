using NUnit.Framework;
using System.Threading.Tasks;
using ExcelPlaywright.TestStep;
using ExcelPlaywright.Utils;
using Microsoft.Playwright;

namespace ExcelPlaywright.Tests
{
    [TestFixture]
    public class UserLoginToLabs : TestBase
    {
        private TestUtils _testUtils;
        private Login _login;

        [OneTimeSetUp]
        public void SetupTest()
        {
            _testUtils = new TestUtils(_page, _browser);
            _login = new Login(_testUtils);
            TestUtils.LoadExcelData("End2End.xlsx", "MDFPraposal");
        }

        [Test, Order(1)]
        public async Task TestLogin()
        {
            await _login.userLogin();
            await _testUtils.SwitchTabAsync(0);
            await _login.NavigateToManageUsers();
            string userName = await _login.GetUserCanLoginAsVSU();
            await _login.UserCanAbleToClickOnSearch();
            await _testUtils.SwitchTabAsync(1);
            await _login.IAgreechecked();
        }
    }
}













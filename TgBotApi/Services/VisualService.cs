using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services
{
    public class VisualService : IVisualService
    {
        public async Task<string> GetByLink(string link)
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(link);

            Screenshot? screenshot = null;
            try
            {
                screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            driver.Quit();

            return screenshot?.AsBase64EncodedString ?? string.Empty;
        }
    }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services
{
    public class VisualService : IVisualService
    {
        public async Task<string> GetByLink(string link)
        {
            var driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444/wd/hub"), new ChromeOptions());

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
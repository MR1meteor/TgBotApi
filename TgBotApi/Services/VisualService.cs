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
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddArgument("headless");
            var driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444/wd/hub"), options);

            driver.Navigate().GoToUrl("https://www.google.com");

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
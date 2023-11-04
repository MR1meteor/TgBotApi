using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services
{
    public class VisualService : IVisualService
    {
        private readonly ILogger<VisualService> logger;

        public VisualService(ILogger<VisualService> logger)
        {
            this.logger = logger;
        }

        public async Task<string> GetByLink(string link)
        {
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddArgument("headless");
            options.AddArguments("--window-size=1440,1080");
            var driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444/wd/hub"), options);

            var normalUrl = System.Web.HttpUtility.UrlDecode(link);

            driver.Navigate().GoToUrl(normalUrl);

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
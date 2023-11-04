using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

            Screenshot? screenshot;

            try
            {
                var normalUrl = System.Web.HttpUtility.UrlDecode(link);

                driver.Navigate().GoToUrl(normalUrl);

                screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            }
            catch (Exception ex)
            {
                driver.Quit();
                logger.LogError(ex.Message, ex.Data);
                throw;
            }

            driver.Quit();

            return screenshot?.AsBase64EncodedString ?? string.Empty;
        }
    }
}
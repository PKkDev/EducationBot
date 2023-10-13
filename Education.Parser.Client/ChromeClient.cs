using Education.Parser.Client.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Education.Parser.Client;

public class ChromeClient : BaseClient
{
    private readonly ChromeOptions _options;

    public ChromeClient(ChromeOptions? options = null, string driverFileName = "chromedriver.exe")
    {
        _options = options ?? new();

        DriverFileName = driverFileName;
        DriverDirecory = Path.Combine(AppContext.BaseDirectory, "drivers");
    }

    public override WebDriver Create()
    {
        var pathToDriver = Path.Combine(DriverDirecory, DriverFileName);

        Driver = new ChromeDriver(pathToDriver, _options, TimeSpan.FromSeconds(60));

        return Driver;
    }

    public override void LocalDispose() { }
}

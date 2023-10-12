﻿using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Education.Parser.Client;

public class EdgeClient : BaseClient
{
    private readonly EdgeOptions _options;

    public EdgeClient(EdgeOptions? options = null, string driverFileName = "msedgedriver.exe")
    {
        _options = options ?? new();
        DriverFileName = driverFileName;
    }

    public override WebDriver Create(string driverFolder)
    {
        var pathToDriver = Path.Combine(driverFolder, DriverFileName);

        Driver = new EdgeDriver(pathToDriver, _options, TimeSpan.FromSeconds(60));

        return Driver;
    }

    public override void LocalDispose() { }
}

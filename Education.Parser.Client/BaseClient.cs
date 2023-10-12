using OpenQA.Selenium;

namespace Education.Parser.Client;

public abstract class BaseClient : IDisposable
{
    protected string DriverFileName;
    public WebDriver Driver;

    public abstract WebDriver Create(string driverFolder);
    public abstract void LocalDispose();

    public void Dispose()
    {
        Driver?.Dispose();

        LocalDispose();
    }
}

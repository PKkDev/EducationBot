using OpenQA.Selenium;

namespace Education.Parser.Client.Base;

public abstract class BaseClient : IDisposable
{
    protected string DriverFileName;
    protected string DriverDirecory;
    public WebDriver Driver;

    public abstract WebDriver Create();
    public abstract void LocalDispose();

    public void Dispose()
    {
        Driver?.Dispose();

        LocalDispose();
    }
}

# CustomViewLocation

 [![Build status](https://ci.appveyor.com/api/projects/status/a3i0semdfaq85my2?svg=true)](https://ci.appveyor.com/project/thisispaulsmith/customviewlocation)

Simple extension to simplify custom view location in ASP.NET Core MVC.

Currently defines `~/Features` and `~/Features/_Shared` as view locations.

## Usage

Update Startup.cs

```csharp
public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        service.AddCustomViewLocation();
        // ...
    } 
    // ...
}
```
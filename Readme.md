# CustomViewLocation

 [![Build status](https://ci.appveyor.com/api/projects/status/a3i0semdfaq85my2?svg=true)](https://ci.appveyor.com/project/thisispaulsmith/customviewlocation) [![MyGet](https://img.shields.io/myget/thisispaulsmith/v/CustomViewLocation.svg?label=myget)](https://www.myget.org/feed/thisispaulsmith/package/nuget/CustomViewLocation) [![NuGet](https://img.shields.io/nuget/v/CustomViewLocation.svg)](https://www.nuget.org/packages/CustomViewLocation)

Simple extension to simplify custom view location in ASP.NET Core MVC.

Currently defines `~/Features` and `~/Features/_Shared` as view locations.

## Usage

Update Startup.cs

```csharp
using NetSmith.AspNetCore.CustomViewLocation;

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
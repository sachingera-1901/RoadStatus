# How to build the application
- Visual Studio
    This is a console application built in .NET 8 and can be loaded and run using Visual Studio 2022 v17.8+.
    Nuget will resolve and download the necessary packages to build the solution. In case of a nuget restore error, the following packages are required for RoadStatus.csproj:
    * Microsoft.Extensions.Configuration Version="8.0.0"
    * Microsoft.Extensions.Hosting Version="8.0.0"
    * Microsoft.Extensions.Http Version="8.0.0"
    * Microsoft.AspNetCore.WebUtilities" Version="8.0.4"

    Packages required for RoadStatus.Tests.csproj are:
    * coverlet.collector Version="6.0.0"
    * Microsoft.NET.Test.Sdk Version="17.8.0"
    * NUnit Version="3.14.0"
    * NUnit.Analyzers Version="3.9.0"
    * NUnit3TestAdapter Version="4.5.0"
    * Moq Version="4.20.70"

- Visual Studio Code
    The application can also be run using Visual Studio Code with the following extensions installed:
        > C# Dev Kit. Available to download at https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit
    Install .NET 8.0 SDK
    Please refer to this page - https://code.visualstudio.com/docs/csharp/get-started

    Open the folder containing the solution and other project files. Run 'dotnet build' in the terminal window.
    This should build the solution and resolve all project dependencies.
    In case of a nuget restore error, please see the list of packages mentioned above.
    A helpful tool to install individual packages is the extension - Nuget Package Manager GUI - available to download at https://marketplace.visualstudio.com/items?itemName=aliasadidev.nugetpackagemanagergui
    Use Ctrl+Shift+P to launch Nuget Package Manager GUI

# How to run unit tests
- Use Test Explorer feature in Visual Studio to identify and run all tests.

- In Visual Studio Code install extension .NET Core Test Explorer to identify and run all tests. Available at https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer#
Refer to this article: https://www.codemag.com/Article/2009101/Interactive-Unit-Testing-with-.NET-Core-and-VS-Code


# Application Configuration in appSettings.json
Before running the application, open appSettings.json file within the project RoadStatus.
Change value for the key AppKey below with your subscription key for Tfl Road API 
'''
{
    "RoadApiUrl" : "https://api.tfl.gov.uk/Road",
    "AppId" : "AppId",
    "AppKey" : "AppKey",
    "Logging": {
        "LogLevel": {
            "Default": "Error",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Warning"
        },
        "Console": {
            "IncludeScopes": true,
            "LogLevel": {
                "Microsoft.Extensions.Hosting": "Warning",
                "Default": "Error"
            }
        }
    }
}
'''
Please note that the default logging for console logging is set to 'Error'. If default value of 'Information' is used, then some Info logs are printed in the console terminal where the application is run, and the desired output appears to be cluttered.

# How to run the application
- Open command prompt
  Change directory to RoadStatus/bin/Debug/net8.0 folder within the folder containing the solution
    * Run command - 'RoadStatus.exe A2' - This should give the road status for road A2
    * Press Ctrl+c to exit
    * Run command 'echo $lastexitcode' - This should return 0 for success or 1 for failure

- In Visual Studio Code, after the application is built successfully, perform the following steps:
    * Right click project RoadStatus and select Open in Integrated Terminal
    * At the command prompt Run command 'dotnet run A2' - This should give the road status for road A2
    * Press Ctrl+c to exit
    * Run command 'echo $lastexitcode' - This should return 0 for success or 1 for failure


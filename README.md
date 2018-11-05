[![NuGet](https://img.shields.io/nuget/v/WorkerOracleTemplate.svg)](https://www.nuget.org/packages/WorkerOracleTemplate/)

# WorkerOracleTemplate
Template for use with the the .NET CLI's dotnet new functionality

# How to install the templates

1. You must have the latest version of the dotnet tooling. This comes with Visual Studio 2017 or from https://dot.net. 
2. Run `dotnet new --install "WorkerOracleTemplate::*"` to install the project template. 
3. Run `dotnet new workeroracle --help` to see how to select the features of the project. 
4. Run `dotnew new workeroracle --name "MyTemplate"` along with any other custom options to create a project from the template.

# Info for `dotnet new` users

You can create new projects with `dotnet new`. For more info take a look at
[Announcing .NET Core Tools Updates in VS 2017 RC](https://blogs.msdn.microsoft.com/dotnet/2017/02/07/announcing-net-core-tools-updates-in-vs-2017-rc/). For further details, check out the [excellent blog posts](http://rehansaeed.com/custom-project-templates-using-dotnet-new/) by Muhammed Rehan Saeed.

# Running the project after it's generated

Once you have created a new project using `dotnet new`, you must restore packages before using `dotnet run`.

```
dotnet new workeroracle --name "MyProject" 
dotnet restore
dotnet run
```

# General info on installing templates

Templates can be installed from packages in any NuGet feed, directories on the file system or ZIP type archives (zip, nupkg, vsix, etc.)
To install a new template use the command:

    dotnet new -i {the path to the folder containing the templates}

# Basic Commands
## Showing help

    dotnet new --help
    dotnet new -h
    dotnet new

## Listing templates

    dotnet new --list
    dotnet new -l
    dotnet new mvc -l            Lists all templates containing the text "mvc"

## Template parameter help

    dotnet new mvc --help
    dotnet new mvc -h

## Template creation

    dotnet new MvcWebTemplate --name MyProject --output src --ParameterName1 Value1 --ParameterName2 Value2 ... --ParameterNameN ValueN
    dotnet new MvcWebTemplate -n MyProject -o src --ParameterName1 Value1 --ParameterName2 Value2 ... --ParameterNameN ValueN

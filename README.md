# DaocClientLib
C# .NET Library for handling Dark Age of Camelot Client resources

This DLL is meant to be linked in any project that needs to access Dark Age of Camelot Client resources for parsing various content.

## Binaries

[![Build status](https://ci.appveyor.com/api/projects/status/moxcav0dfgji12d1?svg=true)](https://ci.appveyor.com/project/dol-leodagan/daocclientlib)

Download : https://github.com/dol-leodagan/DaocClientLib/releases/latest

## How to use

    using DaocClientLib;
    
    public const string daocDirectory = @"C:\Dark Age of Camelot";
				
		public static void Main(string[] args)
		{
		    try
		    {
		        var clientData = new ClientDataWrapper(daocDirectory);
		        // Use some Data...
		        Console.WriteLine(clientData.CraftRecipes[0]);
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("Could not read DAOC setup...");
		        Console.WriteLine(e);
		    }
		}

## How to build

    git clone --recursive https://github.com/dol-leodagan/DaocClientLib
    cd DaocClientLib
    nuget restore

then build with your favorite IDE...

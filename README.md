# DaocClientLib
C# .NET Library for handling Dark Age of Camelot Client ressources

This DLL is meant to be linked in any project that needs to acces Dark Age of Camelot Client ressources for parsing various content.

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

    git clone https://github.com/dol-leodagan/DaocClientLib
    cd DaocClientLib
    nuget restore

then build with your favorite IDE...

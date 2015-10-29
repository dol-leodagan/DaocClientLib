/*
 * DaocClientLib - Dark Age of Camelot Setup Ressources Wrapper
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 dol-leodagan
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using DaocClientLib;

using NUnit.Framework;

namespace DaocClientLib.Test
{
	/// <summary>
	/// ClientDataWrapper NUnit Test Class
	/// </summary>
	[TestFixture]
	public class ClientDataWrapperTest
	{
		#region test setup
		/// <summary>
		/// An Inexisting Path to Test
		/// </summary>
		public static string WrongClientPath { get { return "Z:" + Path.DirectorySeparatorChar + "SomeWrongDir" + Path.DirectorySeparatorChar + "Nowhere to be found"; } }

		/// <summary>
		/// A Temporary Path
		/// </summary>
		public static string TemporaryPath { get { return Environment.CurrentDirectory + Path.DirectorySeparatorChar + "fakeroot"; } }
		
		/// <summary>
		/// An Inexisting Path to Test
		/// </summary>
		public static string WrongClientRoot { get { return Environment.CurrentDirectory + Path.DirectorySeparatorChar + "wrongroot"; } }
		
		/// <summary>
		/// A small Valid MPK with an "existing.txt" file containing "Hello World !"
		/// </summary>
		public static byte[] ValidMPK = {
			0x4D, 0x50, 0x41, 0x4B, 0x02, 0xD8, 0x29, 0xD9,
			0x09, 0x21, 0x05, 0x06, 0x07, 0x1C, 0x09, 0x0A,
			0x0B, 0x0D, 0x0D, 0x0E, 0x0F, 0x78, 0xDA, 0x4B,
			0xAD, 0xC8, 0x2C, 0x2E, 0xC9, 0xCC, 0x4B, 0xD7,
			0xCB, 0x2D, 0xC8, 0x06, 0x00, 0x20, 0x75, 0x04,
			0xE2, 0x78, 0xDA, 0x4B, 0xAD, 0xC8, 0x2C, 0x2E,
			0xC9, 0xCC, 0x4B, 0xD7, 0x2B, 0xA9, 0x28, 0x61,
			0x18, 0x81, 0x40, 0x4D, 0xD4, 0x15, 0xEE, 0x6F,
			0x5E, 0x28, 0x2D, 0x0A, 0xC4, 0xD1, 0x46, 0xBB,
			0xD3, 0x01, 0x88, 0x9A, 0x07, 0xBF, 0x78, 0xDA,
			0xF3, 0x48, 0xCD, 0xC9, 0xC9, 0x57, 0x08, 0xCF,
			0x2F, 0xCA, 0x49, 0x51, 0x50, 0x04, 0x00, 0x20,
			0xA6, 0x04, 0x5E };
		
		/// <summary>
		/// Path where is stored Valid Test MPK
		/// </summary>
		public static string ValidMPKPath { get { return TemporaryPath + Path.DirectorySeparatorChar + "existing.mpk"; } }
		
		/// <summary>
		/// Test CSV String
		/// </summary>
		public static string TestCSV = "Header, Col, Some Tests, \"quoted\",19,87\n0, 1, 2, 3, 4, 5, 6, 7";
		/// <summary>
		/// Expected CSV Result
		/// </summary>
		public static string[][] ExpectedCSV = { new []{ "Header", "Col", "Some Tests", "\"quoted\"", "19", "87", }, new []{ "0", "1", "2", "3", "4", "5", "6", "7", } };
		/// <summary>
		/// Test DAT String
		/// </summary>
		public static string TestDAT = ";comment ... ...\n;some more comments ...\n; comments ; in ; comments ; !\n[region0];come comments ; with comments\n[region1]\nVar=Value ; comment; with comments\nVar2 = Value2[OtherRegion0]\nOtherVar=OtherValue\nOtherVar2 = OtherValue2\n[OtherRegion1]\nOtherVar= OtherParam\nOtherVar2 =OtherParam2";
		/// <summary>
		/// Expected CSV Result
		/// </summary>
		public static Dictionary<string, Dictionary<string, string>> ExpectedDAT = new Dictionary<string, Dictionary<string, string>>{
			{ "region0", new Dictionary<string, string>{ } },
			{ "region1", new Dictionary<string, string>{ {"var", "Value"}, {"var2", "Value2"} } },
			{ "otherregion0", new Dictionary<string, string>{ {"othervar", "OtherValue"}, {"othervar2", "OtherValue2"} } },
			{ "otherregion1", new Dictionary<string, string>{ {"othervar", "OtherParam"}, {"othervar2", "OtherParam2"} } },
		};
		
		/// <summary>
		/// Set up some testing directory
		/// </summary>
		[SetUp]
    	public void Init()
    	{
    		if (!Directory.Exists(TemporaryPath))
    			Directory.CreateDirectory(TemporaryPath);
    		
    		if (!Directory.Exists(WrongClientRoot))
	    		Directory.CreateDirectory(WrongClientRoot);
    		
    		using (var s = File.Create(TemporaryPath + Path.DirectorySeparatorChar + "camelot.exe"))
    		{
    		}
    		
    		using (var s = File.Create(TemporaryPath + Path.DirectorySeparatorChar + "game.dll"))
    		{
    		}
    		
    		using (var s = File.Create(ValidMPKPath))
    		{
    			s.Write(ValidMPK, 0, ValidMPK.Length);
    		}
    	}
		
    	#endregion
    	
    	#region test ClientDataWrapper
		/// <summary>
		/// Test Loading a null directory for Argument Exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestWrongDirectoryLoading()
		{
			string str = null;
			var test = new ClientDataWrapper(str);
		}
		
		/// <summary>
		/// Test Loading an inexisting directory Path for File Exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void TestWrongPathDirectoryLoading()
		{
			var test = new ClientDataWrapper(WrongClientPath);
		}
		
		/// <summary>
		/// Test Loading a wrong directory root for Argument Exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void TestWrongPathRootLoading()
		{
			var test = new ClientDataWrapper(WrongClientRoot);
		}
		
		/// <summary>
		/// Test Loading a valid directory root
		/// </summary>
		[Test]
		public void TestValidPathRootLoading()
		{
			var test = new ClientDataWrapper(TemporaryPath);
			
			Assert.IsTrue(test.ClientFiles.Any(f => f.Name.Equals("camelot.exe", StringComparison.OrdinalIgnoreCase)));
			Assert.IsTrue(test.ClientFiles.Any(f => f.Name.Equals("game.dll", StringComparison.OrdinalIgnoreCase)));
		}
    	#endregion
    	
    	#region test GetFileDataFromPackageExtension
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with null FileInfo Array
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionGetDataNullArgumentFiles()
    	{
    		FileInfo[] test = null;
    		test.GetFileDataFromPackage(null, null);
    	}
    	    	
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with null Package Name
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionGetDataNullArgumentPackage()
    	{
    		var test = new FileInfo[0];
    		test.GetFileDataFromPackage(null, null);
    	}
    	    	
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with null Package File name
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionGetDataNullArgumentPackageFile()
    	{
    		var test = new FileInfo[0];
    		test.GetFileDataFromPackage("none.mpk", null);
    	}
    	
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with wrong MPK Path
    	/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
    	public void TestExtensionGetDataMissingPackage()
    	{
    		var test = new FileInfo[0];
    		test.GetFileDataFromPackage("none.mpk", "none.txt");
    	}
    	
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with Valid MPK but missing File 
    	/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
    	public void TestExtensionGetDataMissingPackageFile()
    	{
    		var mpk = new FileInfo(ValidMPKPath);
    		var test = new []{ mpk };
    		test.GetFileDataFromPackage(mpk.Name, "none.txt");
    	}
    	
    	/// <summary>
    	/// Test GetFileDataFromPackageExtension with valid Arrray and MPK Package to Get the Test String in a file.
    	/// </summary>
		[Test]
    	public void TestExtensionGetDataPackageFile()
    	{
    		var mpk = new FileInfo(ValidMPKPath);
    		var test = new []{ mpk };
    		var check = test.GetFileDataFromPackage(mpk.Name, "existing.txt");
    		Assert.AreEqual("Hello World !", System.Text.Encoding.UTF8.GetString(check));
    	}
    	
    	#endregion
    	
    	#region test GetAllFileDataFromPackage
    	/// <summary>
    	/// Test GetAllFileDataFromPackage with null File Array
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionGetAllDataNullArgumentFiles()
    	{
    		FileInfo[] test = null;
    		test.GetAllFileDataFromPackage(null);
    	}
    	
    	/// <summary>
    	/// Test GetAllFileDataFromPackage with null Package Name
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionGetAllDataNullArgumentPackage()
    	{
    		var test = new FileInfo[0];
    		test.GetAllFileDataFromPackage(null);
    	}
    	
    	/// <summary>
    	/// Test GetAllFileDataFromPackage with Missing Package
    	/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
    	public void TestExtensionGetAllDataMissingPackage()
    	{
    		var test = new FileInfo[0];
    		test.GetAllFileDataFromPackage("none.mpk");
    	}
    	
    	/// <summary>
    	/// Test GetAllFileDataFromPackage with Valid Package and Get Data
    	/// </summary>
		[Test]
    	public void TestExtensionGetAllDataPackage()
    	{
    		var mpk = new FileInfo(ValidMPKPath);
    		var test = new []{ mpk };
    		var check = test.GetAllFileDataFromPackage(mpk.Name);
    		
    		Assert.IsTrue(check.ContainsKey("existing.txt"));
    		Assert.AreEqual("Hello World !", System.Text.Encoding.UTF8.GetString(check["existing.txt"]));
    		
    	}
    	#endregion
    	
    	#region test ReadCSVFile
    	/// <summary>
    	/// Test ReadCSVFile with null Byte Array
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionReadCSVNullArgumentFile()
    	{
    		byte[] test = null;
    		test.ReadCSVFile();
    	}
    	
    	/// <summary>
    	/// Test ReadCSVFile with valid Byte Array
    	/// </summary>
		[Test]
    	public void TestExtensionReadCSVFile()
    	{
    		var test = System.Text.Encoding.UTF8.GetBytes(TestCSV).ReadCSVFile();
    		
    		for (int i = 0 ; i < test.Length ; i++)
    		{
    			for (int j = 0 ; j < test[i].Length ; j++)
    			{
    				Assert.AreEqual(ExpectedCSV[i][j], test[i][j]);
    			}
    		}
    	}  	
    	#endregion
    	
    	#region test ReadDATFile
    	/// <summary>
    	/// Test ReadDATFile with null Byte Array
    	/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
    	public void TestExtensionReadDATNullArgumentFile()
    	{
    		byte[] test = null;
    		test.ReadDATFile();
    	}
    	
    	/// <summary>
    	/// Test ReadDATFile with null Byte Array
    	/// </summary>
		[Test]
    	public void TestExtensionReadDATFile()
    	{
    		var test = System.Text.Encoding.UTF8.GetBytes(TestDAT).ReadDATFile();
    		
    		Assert.AreEqual(test.Count, ExpectedDAT.Count);
    		
    		foreach (var key in ExpectedDAT.Keys)
    		{
    			Assert.IsTrue(test.ContainsKey(key));
    			Assert.AreEqual(ExpectedDAT[key].Count, test[key].Count);
    			
    			foreach (var subkey in ExpectedDAT[key].Keys)
    			{
    				Assert.IsTrue(test[key].ContainsKey(subkey));
    				Assert.AreEqual(ExpectedDAT[key][subkey], test[key][subkey]);
    			}
    		}
    	}
    	#endregion
    	
		public ClientDataWrapperTest()
		{
		}
	}
}

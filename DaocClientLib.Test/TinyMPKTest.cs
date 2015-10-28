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

using DaocClientLib.MPK;

using NUnit.Framework;

namespace DaocClientLib.Test
{
	/// <summary>
	/// TinyMPK NUnit Test Class
	/// </summary>
	public class TinyMPKTest
	{
		#region setup
		
		/// <summary>
		/// Path where is stored Valid Test MPK
		/// </summary>
		public static string ValidMPKPath { get { return ClientDataWrapperTest.TemporaryPath + Path.DirectorySeparatorChar + "validmpktest.mpk"; } }
		
		/// <summary>
		/// Path where is stored a not Valid Test MPK
		/// </summary>
		public static string WrongMPKPath { get { return ClientDataWrapperTest.TemporaryPath + Path.DirectorySeparatorChar + "wrongmpktest.mpk"; } }
		
		/// <summary>
		/// Path where is stored a Short Test MPK
		/// </summary>
		public static string ShortMPKPath { get { return ClientDataWrapperTest.TemporaryPath + Path.DirectorySeparatorChar + "shortmpktest.mpk"; } }

		/// <summary>
		/// Path where is stored a Truncated Test MPK
		/// </summary>
		public static string TruncatedMPKPath { get { return ClientDataWrapperTest.TemporaryPath + Path.DirectorySeparatorChar + "truncatedmpktest.mpk"; } }

		/// <summary>
		/// Set up some testing files
		/// </summary>
		[SetUp]
		public void Init()
		{
    		if (!Directory.Exists(ClientDataWrapperTest.TemporaryPath))
    			Directory.CreateDirectory(ClientDataWrapperTest.TemporaryPath);
    		
    		using (var s = File.Create(ValidMPKPath))
    		{
    			s.Write(ClientDataWrapperTest.ValidMPK, 0, ClientDataWrapperTest.ValidMPK.Length);
    		}
    		using (var s = File.Create(WrongMPKPath))
    		{
    			s.Write(new byte[]{0, 0, 0, 0}, 0, 4);
    		}
    		using (var s = File.Create(ShortMPKPath))
    		{
    			s.Write(ClientDataWrapperTest.ValidMPK, 0, 1);
    		}
    		using (var s = File.Create(TruncatedMPKPath))
    		{
    			s.Write(ClientDataWrapperTest.ValidMPK, 0, ClientDataWrapperTest.ValidMPK.Length / 2);
    		}
		}
		#endregion
		
		#region test Mpk Constuctor
		/// <summary>
		/// Test Constructor with Null FileInfo
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestMPKConstructorNullFileArgument()
		{
			FileInfo test = null;
			new TinyMPK(test);
		}
		
		/// <summary>
		/// Test Constructor with Null path string
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestMPKConstructorNullStringArgument()
		{
			string test = null;
			new TinyMPK(test);
		}
		
		/// <summary>
		/// Test Constructor with Invalid path string
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestMPKConstructorWrongPathArgument()
		{
			string test = "";
			new TinyMPK(test);
		}
		
		/// <summary>
		/// Test Constructor with not existing path
		/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void TestMPKConstructorMissingPathArgument()
		{
			string test = "notexisting.mpk";
			new TinyMPK(test);
		}
		
		/// <summary>
		/// Test Constructor with a file not being an MPK Archive
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestMPKConstructorWrongMPKArgument()
		{
			new TinyMPK(WrongMPKPath);
		}
		
		/// <summary>
		/// Test Constructor with a shorter file than magic byte
		/// </summary>
		[Test]
		[ExpectedException(typeof(EndOfStreamException))]
		public void TestMPKConstructorShortMPKArgument()
		{
			new TinyMPK(ShortMPKPath);
		}
		
		/// <summary>
		/// Test Constructor with a truncated file
		/// </summary>
		[Test]
		[ExpectedException(typeof(FileLoadException))]
		public void TestMPKConstructorTruncatedMPKArgument()
		{
			new TinyMPK(TruncatedMPKPath);
		}
		
		/// <summary>
		/// Test Constructor with valid test file and check members
		/// </summary>
		[Test]
		public void TestMPKConstructor()
		{
			var test = new TinyMPK(ValidMPKPath);
			
			Assert.AreEqual(1, test.Count);
			Assert.NotNull(test["existing.txt"]);
			Assert.NotNull(test["existing.txt"].Data);
			Assert.Null(test["not existing at all.not"]);
			Assert.AreEqual("existing.txt", test["existing.txt"].Name);
			
			var expected = System.Text.Encoding.UTF8.GetBytes("Hello World !");
			
			for (int i = 0 ; i < test["existing.txt"].Data.Length ; i++)
				Assert.AreEqual(expected[i], test["existing.txt"].Data[i]);
		}
		#endregion
		
		public TinyMPKTest()
		{
		}
	}
}

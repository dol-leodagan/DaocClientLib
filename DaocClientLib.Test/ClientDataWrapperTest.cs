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
using System.IO;

using DaocClientLib;

using NUnit.Framework;

namespace DaocClientLib.Test
{
	/// <summary>
	/// ClientDataWrapperTest NUNIT Test Class
	/// </summary>
	[TestFixture]
	public class ClientDataWrapperTest
	{
		/// <summary>
		/// An Inexisting Path to Test
		/// </summary>
		public string WrongClientPath { get { return "Z:" + Path.PathSeparator + "SomeWrongDir" + Path.PathSeparator + "Nowhere to be found"; } }
		
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
		
		public ClientDataWrapperTest()
		{
		}
	}
}

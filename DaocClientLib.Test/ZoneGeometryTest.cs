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
	/// ZoneGeometry NUnit Test Class
	/// </summary>
	public class ZoneGeometryTest
	{
		#region setup
		[SetUp]
		public void Init()
		{
    		if (!Directory.Exists(ClientDataWrapperTest.TemporaryPath))
    			Directory.CreateDirectory(ClientDataWrapperTest.TemporaryPath);

    		using (var s = File.Create(ClientDataWrapperTest.ValidMPKPath))
    		{
    			s.Write(ClientDataWrapperTest.ValidMPK, 0, ClientDataWrapperTest.ValidMPK.Length);
    		}
		}
		#endregion
		
		#region tests
		/// <summary>
		/// Test Constructor with null File Array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestZoneGeometryWithNullSectorPack()
		{
			FileInfo[] test = null;
			new ZoneGeometry(0, test);
		}
		/// <summary>
		/// Test Constructor with empty File Array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestZoneGeometryWithMissingSectorPack()
		{
			new ZoneGeometry(0, new FileInfo[0]);
		}
		/// <summary>
		/// Test Constructor with missing Sector.dat Package (not zone...)
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestZoneGeometryWithEmptySectorPack()
		{
			new ZoneGeometry(0, new FileInfo[] { new FileInfo(ClientDataWrapperTest.ValidMPKPath) } );
		}
		
		#endregion
		public ZoneGeometryTest()
		{
		}
	}
}

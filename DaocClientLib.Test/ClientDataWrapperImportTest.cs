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
	/// ClientDataWrapper NUnit Test Class for Import methods
	/// </summary>
	public class ClientDataWrapperImportTest
	{
		#region setup
		[SetUp]
		public void Init()
		{
			
		}
		#endregion
		#region test treemap
		/// <summary>
		/// Test Treemap Constructor with null Tree Map Array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TreeMapImportWithNullTreeMapArgument()
		{
			new TreeReplacementMap(null, null);
		}
		/// <summary>
		/// Test Treemap Constructor with null Cluster Map Array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TreeMapImportWithNullClusterMapArgument()
		{
			new TreeReplacementMap(new string[0][], null);
		}
		/// <summary>
		/// Test Treemap Constructor with wrong Tree Map array
		/// </summary>
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void TreeMapImportWithWrongTreeMapArgument()
		{
			var content = new string[][] {
				new []{ "test", "wrong", "field", "but", "good", "count" },
			};
			var test = new TreeReplacementMap(content, new string[0][]);
			
			Assert.IsFalse(test.Keys.Contains("test"));
			throw test.Warnings.First();
		}
		/// <summary>
		/// Test Treemap Constructor with wrong Cluster Map array
		/// </summary>
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void TreeMapImportWithWrongClusterMapArgument()
		{
			var content = new string[][] {
				new []{ "test", "good", "field", "short", "-32000", },
			};
			var cluster = new string[][] {
				new [] { "clustertest", "no values", },
			};
			var test = new TreeReplacementMap(content, cluster);
			
			Assert.IsTrue(test.Keys.Contains("test"));
			Assert.IsFalse(test.Keys.Contains("clustertest"));
			throw test.Warnings.First();
		}
		/// <summary>
		/// Test Treemap Constructor with Valid Data
		/// </summary>
		[Test]
		public void TreeMapImportWithValidArgument()
		{
			var content = new string[][] {
				new []{ "NIF Name","Replacement Tree","Bark Texture","Leaf Texture","zoffset","scale", },
				new []{ "test", "good", "field", "short", "-32000", },
			};
			var cluster = new string[][] {
				new [] { "clustertest", "test", "32", "64", "128", "256", "512", "1024"},
				new [] { "name", "test", "32", "64", "128", "256", "512", "1024"},
			};
			var test = new TreeReplacementMap(content, cluster);
			
			Assert.IsTrue(test.Keys.Contains("test"));
			Assert.IsTrue(test.Keys.Contains("clustertest"));
			
			Assert.IsFalse(test.Keys.Contains("nif name"));
			Assert.IsFalse(test.Keys.Contains("name"));
			
			Assert.IsTrue(test["test"].Count() == 1);
			Assert.IsTrue(test["clustertest"].Count() == 2);

			Assert.IsTrue(test["test"].All(data => data.OffsetZ.Equals(-32000f)));
			Assert.IsTrue(test["clustertest"].Any(data => data.RealNif == "test" && data.OffsetZ.Equals(-32000f + 128f) && data.OffsetX.Equals(32f) && data.OffsetY.Equals(64f)));
			Assert.IsTrue(test["clustertest"].Any(data => data.RealNif == "test" && data.OffsetZ.Equals(-32000f + 1024f) && data.OffsetX.Equals(256f) && data.OffsetY.Equals(512f)));
			
			if (test.Warnings.Any())
				throw test.Warnings.First();
		}
		#endregion
		#region test jumppoints
		/// <summary>
		/// Test Jump Point Constructor with null Argument
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void JumpPointsImportWithNullArgument()
		{
			new JumpPoint(null);
		}
		/// <summary>
		/// Test Jump Point Constructor with invalid Argument
		/// </summary>
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void JumpPointsImportWithInvalidArgument()
		{
			var test = new []{ "test", };
			new JumpPoint(test);
		}
		/// <summary>
		/// Test Jump Point Constructor with Invalid Parsed Values
		/// </summary>
		[Test]
		[ExpectedException(typeof(FormatException))]
		public void JumpPointsImportWithInvalidValues()
		{
			var test = new []{ "test","test","test","test","test","test","test", };
			new JumpPoint(test);
		}
		/// <summary>
		/// Test Jump Point Constructor with valid data, and invalid optional data
		/// </summary>
		[Test]
		public void JumpPointsImportWithValidValues()
		{
			var test = new []{ "1","test", "2", "1000","2000","3000","180","test","22", "222", };
			var obj = new JumpPoint(test);
			
			Assert.AreEqual(1, obj.ID);
			Assert.AreEqual("test", obj.Name);
			Assert.AreEqual(2, obj.ZoneID);
			Assert.AreEqual(1000, obj.X);
			Assert.AreEqual(2000, obj.Y);
			Assert.AreEqual(3000, obj.Z);
			Assert.AreEqual(180, obj.HeadingDegrees);
			Assert.AreEqual(2, obj.AlsoInZoneID.Length);
			Assert.IsTrue(obj.AlsoInZoneID.Any(z => z == 22));
			Assert.IsTrue(obj.AlsoInZoneID.Any(z => z == 222));
		}
		#endregion
		
		public ClientDataWrapperImportTest()
		{
		}
	}
}

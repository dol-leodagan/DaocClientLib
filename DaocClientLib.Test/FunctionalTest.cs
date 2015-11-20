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
	/// Description of FunctionalTest.
	/// </summary>
	[TestFixture]
	public class FunctionalTest
	{
		protected ClientDataWrapper client;
		protected DirectoryInfo extract;
		protected string loc = @"C:\Dark Age of Camelot1118L";
		
		[TestFixtureSetUp]
		public void Init()
		{
			client = new ClientDataWrapper(loc);
			extract = new DirectoryInfo(@"D:\extract\");
		}
		
		[Test, Explicit]
		public void ExtractAllClientPackage()
		{
			foreach (var file in client.ClientFiles.Where(f => f.Extension.Equals(".mpk") || f.Extension.Equals(".npk")))
			{
				var path = extract.FullName + file.Directory.FullName.Substring(loc.Length);
				Directory.CreateDirectory(path);
				foreach (var packaged in file.GetAllFileDataFromPackage())
				{
					File.WriteAllBytes(path + Path.DirectorySeparatorChar + packaged.Key, packaged.Value);
				}
			}
		}
		
		[Test, Explicit]
		public void TestClientZoneEdgeCases()
		{
			var zones = client.ZonesData;
			var geometries = client.ZonesGeometry;
			var results = new List<NifGeometry>();
			foreach(var zone in zones.Where(zn => !zn.IsProxyZone))
			{
				var id = zone.ID;
				var geometry = geometries[id];
				
				if (geometry == null)
					continue;
				
				if (geometry.IsCity)
					continue;
				
				NifGeometry[] geoms;
				if (geometry.IsDungeon)
					geoms = geometry.DungeonPlaces;
				else
					geoms = geometry.TerrainFixtures;
				
				var withFlips = geoms.Where(nif => nif.Flip).ToArray();
				
				results.AddRange(withFlips);
				
				if (withFlips.Any())
					continue;
			}
			
			if (results.Count > 0)
				return;
		}
		
		public FunctionalTest()
		{
		}
	}
}

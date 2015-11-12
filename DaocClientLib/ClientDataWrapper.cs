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
using System.Linq;
using System.Text.RegularExpressions;

using DaocClientLib.MPK;

namespace DaocClientLib
{
	/// <summary>
	/// Main Class to build a Ressources Tree upon a Dark Age of Camelot setup directory.
	/// </summary>
	public class ClientDataWrapper
	{
		/// <summary>
		/// File loading Filters. (Regex)
		/// </summary>
		private readonly string[] m_fileFilters;
		
		/// <summary>
		/// File loading Filters. (Regex)
		/// </summary>
		public string[] FileFilters { get { return m_fileFilters; } }
		
		/// <summary>
		/// File loading Filters. (Regex)
		/// </summary>
		public static string[] DefaultFilters { get { return new []{ @"camelot\.exe", @"game\.dll", @".+\.mpk|.+\.npk", @".+\.nif|.+\.nhd", @".+\.pcx|.+\.tga|.+\.bmp|.+\.dds", @".+\.csv" }; } }
		
		/// <summary>
		/// Client Files Detected
		/// </summary>
		private readonly FileInfo[] m_clientFiles;

		/// <summary>
		/// Client Files Detected
		/// </summary>
		public FileInfo[] ClientFiles { get { return m_clientFiles; }}
				
		/// <summary>
		/// Create a Client Data Wrapper from Directory Path
		/// </summary>
		/// <param name="path"></param>
		public ClientDataWrapper(string path)
			: this(new DirectoryInfo(path))
		{
		}

		/// <summary>
		/// Create a Client Data Wrapper from Directory Path and given Regex File Filters
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filters"></param>
		public ClientDataWrapper(string path, string[] filters)
			: this(new DirectoryInfo(path), filters)
		{
		}
		
		/// <summary>
		/// Create a Client Data Wrapper from Directory Path
		/// </summary>
		/// <param name="path"></param>
		public ClientDataWrapper(DirectoryInfo path)
			: this(path, null)
		{
		}
		
		/// <summary>
		/// Create a Client Data Wrapper from Directory Path and given Regex File Filters
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filters"></param>
		public ClientDataWrapper(DirectoryInfo path, string[] filters)
		{
			if (filters != null)
				m_fileFilters = filters;
			else
				m_fileFilters = DefaultFilters;
			
			if (path == null)
				throw new ArgumentNullException("path");
						
			if (!path.Exists)
				throw new FileNotFoundException("Could not Find Daoc Client Directory !", path.FullName);
			
			if (!path.EnumerateFiles().Any(f => f.Name.Equals("camelot.exe", StringComparison.OrdinalIgnoreCase)))
				throw new NotSupportedException(string.Format("camelot.exe could not be found, make sure directory {0} is a client setup !", path.FullName));
			if (!path.EnumerateFiles().Any(f => f.Name.Equals("game.dll", StringComparison.OrdinalIgnoreCase)))
				throw new NotSupportedException(string.Format("game.dll could not be found, make sure directory {0} is a client setup !", path.FullName));
			
			// Filter All files recursively against Regex strings
			m_clientFiles = path.GetFiles("*", SearchOption.AllDirectories)
				.Where(f => m_fileFilters.Any(r => Regex.IsMatch(f.Name, r))).ToArray();
		}
		
		#region global accessors
		const string CraftPackage = "ifd.mpk";
		const string CraftFile = "tdl.crf";
		
		/// <summary>
		/// Retrieve Client Craft Recipes Data
		/// </summary>
		public virtual CraftDataRecipe[] CraftRecipes { get { return CraftListData.RecipesFromFileBytes(m_clientFiles.GetFileDataFromPackage(CraftPackage, CraftFile)); } }
		
		const string ZonesDatPackage = "zones.mpk";
		const string ZonesDatFile = "zones.dat";
		
		/// <summary>
		/// Retrieve Client Zones List Data
		/// </summary>
		public virtual ZoneData[] ZonesData { get { return ZoneDataList.ZonesFromFileBytes(m_clientFiles.GetFileDataFromPackage(ZonesDatPackage, ZonesDatFile)); } }
		
		/// <summary>
		/// Retrieve Zone Geometry Index
		/// </summary>
		public virtual ZoneGeometryChooser ZonesGeometry { get { return new ZoneGeometryChooser(this); } }
		
		const string TreeMapPackage = "treemap.mpk";
		const string TreeMapFile = "treemap.csv";
		const string TreeClusterPackage = "tree_clusters.mpk";
		const string TreeClusterFile = "tree_clusters.csv";
		
		/// <summary>
		/// Retrieve Client Tree Replacement Map
		/// </summary>
		public virtual TreeReplacementMap TreeReplacement
		{
			get
			{
				return new TreeReplacementMap(m_clientFiles.GetFileDataFromPackage(TreeMapPackage, TreeMapFile).ReadCSVFile(),
				                              m_clientFiles.GetFileDataFromPackage(TreeClusterPackage, TreeClusterFile).ReadCSVFile());
			}
		}
		
		const string JumpPointsPackage = "gamedata.mpk";
		const string JumpPointsFile = "jumppoints.csv";
		
		/// <summary>
		/// Retrieve Hard coded /Jump Points
		/// </summary>
		public virtual JumpPoint[] JumpPoints { get { return m_clientFiles.GetFileDataFromPackage(JumpPointsPackage, JumpPointsFile).ReadCSVFile().Select(line => new JumpPoint(line)).ToArray(); } }
		#endregion
		
		#region files accessors
		/// <summary>
		/// Try to Retrieve File resources from arbitrary name 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="packageExt"></param>
		/// <param name="fileExt"></param>
		/// <returns>byte array file or null if not found</returns>
		public byte[] SearchRawFileOrPackaged(string name, string[] packageExt, string[] fileExt)
		{
			var file = m_clientFiles.FirstOrDefault(f => f.Name.Substring(0, f.Name.Length - f.Extension.Length).Equals(name, StringComparison.OrdinalIgnoreCase) && fileExt.Any(ext => ext.Equals(f.Extension, StringComparison.OrdinalIgnoreCase)));
			// Found as Raw File
			if (file != null)
				return File.ReadAllBytes(file.FullName);
			
			var packages = m_clientFiles.Where(f => f.Name.Substring(0, f.Name.Length - f.Extension.Length).Equals(name, StringComparison.OrdinalIgnoreCase) && packageExt.Any(ext => ext.Equals(f.Extension, StringComparison.OrdinalIgnoreCase)));
			// Found as Packaged File
			foreach (var package in packages)
			{
				var mpk = new TinyMPK(package);
				var mpkfile = mpk.Select(kv => kv.Key).FirstOrDefault(ent => fileExt.Any(ext => string.Format("{0}{1}", name, ext).Equals(ent, StringComparison.OrdinalIgnoreCase)));
				if (mpkfile != null)
					return mpk[mpkfile].Data;
			}
			
			return null;
		}
		
		/// <summary>
		/// Try to Retrieve File resources from arbitrary name 
		/// </summary>
		/// <param name="nameExt"></param>
		/// <param name="packageExt"></param>
		/// <returns>byte array file or null if not found</returns>
		public byte[] SearchRawFileOrPackaged(string nameExt, string[] packageExt)
		{
			var file = m_clientFiles.FirstOrDefault(f => f.Name.Equals(nameExt, StringComparison.OrdinalIgnoreCase));
			// Found as Raw File
			if (file != null)
				return File.ReadAllBytes(file.FullName);
			
			// Search package with file name only
			var nameInfo = new FileInfo(nameExt);
			var name = nameInfo.Name.Substring(0, nameInfo.Name.Length - nameInfo.Extension.Length);
			
			var packages = m_clientFiles.Where(f => f.Name.Substring(0, f.Name.Length - f.Extension.Length).Equals(name, StringComparison.OrdinalIgnoreCase) && packageExt.Any(ext => ext.Equals(f.Extension, StringComparison.OrdinalIgnoreCase)));
			// Found as Packaged File
			foreach (var package in packages)
			{
				var mpk = new TinyMPK(package);
				var mpkfile = mpk.Select(kv => kv.Key).FirstOrDefault(ent => nameExt.Equals(ent, StringComparison.OrdinalIgnoreCase));
				if (mpkfile != null)
					return mpk[mpkfile].Data;
			}
			
			return null;
		}
		
		#endregion
	}
}

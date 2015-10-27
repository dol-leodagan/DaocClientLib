/*
 * DaocClientLib - Dark Age of Camelot Setup Ressources Wrapper
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
				m_fileFilters = new []{ @"camelot\.exe", @"game\.dll", @".+\.mpk|.+\.npk", @".+\.nif|.+\.nhd", @".+\.pcx|.+\.tga|.+\.bmp|.+\.dds", @".+\.csv" };
			
			if (path == null)
				throw new ArgumentNullException("path");
						
			if (!path.Exists)
				throw new FileNotFoundException("Could not Find Daoc Client Directory !", path.FullName);
			
			// Filter All files recursively against Regex strings
			m_clientFiles = path.GetFiles("*", SearchOption.AllDirectories)
				.Where(f => m_fileFilters.Any(r => Regex.IsMatch(f.Name, r))).ToArray();
		}
	}
}

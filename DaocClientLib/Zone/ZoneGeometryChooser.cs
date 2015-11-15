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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DaocClientLib
{
	/// <summary>
	/// ZoneGeometryChooser indexes all Zones Geometry Data
	/// </summary>
	public class ZoneGeometryChooser
	{
		/// <summary>
		/// Zone Suffix Regex
		/// </summary>
		public const string ZoneRegEx =  @"\d{1,3}";
		/// <summary>
		/// Zone Prefix
		/// </summary>
		public const string ZonePrefix =  "zone";
		/// <summary>
		/// Zone Directories indexed by ID
		/// </summary>
		protected readonly Dictionary<int, FileInfo[]> m_zoneDict;
		
		/// <summary>
		/// Zone Type indexed by ID
		/// </summary>
		protected readonly Dictionary<int, ZoneType> m_zoneTypeDict;
		
		/// <summary>
		/// Get Geometry Data From Zone Index
		/// </summary>
		public virtual ZoneGeometry this[int Index]
		{
			get
			{
				FileInfo[] files;
				ZoneType type;
				if (m_zoneTypeDict.TryGetValue(Index, out type))
					return m_zoneDict.TryGetValue(Index, out files) ? new ZoneGeometry(Index, files, type) : null;
				
				return null;
			}
		}
		
		public ZoneGeometryChooser(ClientDataWrapper client)
		{
			m_zoneDict = client.ClientFiles.Where(f => Regex.IsMatch(f.Directory.Name, string.Format("^{0}{1}$", ZonePrefix, ZoneRegEx), RegexOptions.IgnoreCase))
				.GroupBy(f => f.Directory.Name)
				.ToDictionary(k => Convert.ToInt32(new string(k.Key.Skip(ZonePrefix.Length).ToArray())), k => k.ToArray());
			
			m_zoneTypeDict = client.ZonesData.ToDictionary(k => (int)k.ID, v => (ZoneType)v.Type);
		}
	}
}

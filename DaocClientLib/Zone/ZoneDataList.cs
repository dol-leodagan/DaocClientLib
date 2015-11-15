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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DaocClientLib
{
	/// <summary>
	/// Client Zone Data List
	/// </summary>
	public sealed class ZoneDataList
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
		/// Zone Data Collection
		/// </summary>
		public ZoneData[] Zones { get; private set; }
		
		/// <summary>
		/// Warnings during Parse
		/// </summary>
		private List<Exception> m_Warnings = new List<Exception>();
		
		/// <summary>
		/// Get Warnings that occured during parse 
		/// </summary>
		public Exception[] Warnings { get { return m_Warnings.ToArray(); } }
		
		/// <summary>
		/// Zone Data List from Parsed DAT Content
		/// </summary>
		/// <param name="content"></param>
		public ZoneDataList(IDictionary<string, IDictionary<string, string>> content)
		{
			Zones = content.Where(kv => Regex.IsMatch(kv.Key, string.Format("^{0}{1}$", ZonePrefix, ZoneRegEx), RegexOptions.IgnoreCase))
				.Select(kv => {
				        	try
				        	{
				        		return new ZoneData(kv.Key, kv.Value);
				        	}
				        	catch (Exception e)
				        	{
				        		m_Warnings.Add(new NotSupportedException(string.Format("Could not parse Zone Data '{0}' from List", kv.Key), e));
				        		return null;
				        	}
				        }).Where(val => val != null).ToArray();
		}
		
		/// <summary>
		/// Retrieve a ZoneData Collection From a File Bytes Array
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static ZoneData[] ZonesFromFileBytes(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			
			if (input.Length == 0)
				throw new ArgumentException("Input Array is Empty", "input");

			return new ZoneDataList(input.ReadDATFile()).Zones;
		}
	}
}

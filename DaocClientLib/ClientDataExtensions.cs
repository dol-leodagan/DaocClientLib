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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DaocClientLib.MPK;

namespace DaocClientLib
{
	/// <summary>
	/// Extensions Utils for ClientDataWrapper
	/// </summary>
	public static class ClientDataExtensions
	{
		/// <summary>
		/// Convert Byte Array from CSV File into a Line/Column Array of string
		/// </summary>
		/// <param name="infile"></param>
		/// <returns></returns>
		public static string[][] ReadCSVFile(this byte[] infile)
		{
			if (infile == null)
				throw new ArgumentNullException("infile");
					
			var content = System.Text.Encoding.UTF8.GetString(infile);
			
			return Regex.Matches(content, @"^.+$", RegexOptions.Multiline).OfType<Match>().Select(m => m.Value.Trim().Split(',').Select(s => s.Trim()).ToArray()).ToArray();
		}
		
		/// <summary>
		/// Convert Byte Array from DAT File (Ini Type) into a composed Dictionary of Region/Variable=Value 
		/// </summary>
		/// <param name="infile"></param>
		/// <returns></returns>
		public static IDictionary<string, IDictionary<string, string>> ReadDATFile(this byte[] infile)
		{
			if (infile == null)
				throw new ArgumentNullException("infile");

			var content = System.Text.Encoding.UTF8.GetString(infile);
			
			
			return Regex.Matches(Regex.Replace(content, @";(.*)$", ""), @"\[(?<index>.+?)\](?<params>[^\[]+)", RegexOptions.Multiline).OfType<Match>()
				.Select(region =>
				        new KeyValuePair<string, IDictionary<string, string>>(region.Groups["index"].Value.Trim().ToLower(),
				                                                              Regex.Matches(region.Groups["params"].Value, @"(?<name>[a-z0-9'_\-\. ]+)=(?<value>[a-z0-9'_\-\. ]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).OfType<Match>()
				                                                              .Select(sub =>
				                                                                      new KeyValuePair<string, string>(sub.Groups["name"].Value.Trim().ToLower(),
				                                                                                                       sub.Groups["value"].Value.Trim()))
				                                                              .ToDictionary(k => k.Key, v => v.Value)
				                                                             )
				       ).ToDictionary(k => k.Key, v => v.Value);
		}
		
		/// <summary>
		/// Extract a File from MPK package 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="packageName"></param>
		/// <param name="fileName"></param>
		/// <returns>File Byte Array</returns>
		public static byte[] GetFileDataFromPackage(this ClientDataWrapper data, string packageName, string fileName)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (string.IsNullOrEmpty(packageName))
				throw new ArgumentNullException("packageName");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			
			var pack = data.ClientFiles.FirstOrDefault(file => file.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase));
			
			if(pack == null)
				throw new System.IO.FileNotFoundException("Package could not be found !", packageName);
			
			return new TinyMPK(pack.FullName)[fileName].Data;
		}
		
		/// <summary>
		/// Extract a File from MPK package 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="packageName"></param>
		/// <returns>File Byte Array</returns>
		public static IDictionary<string, byte[]> GetAllFileDataFromPackage(this ClientDataWrapper data, string packageName)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (string.IsNullOrEmpty(packageName))
				throw new ArgumentNullException("packageName");
			
			var pack = data.ClientFiles.FirstOrDefault(file => file.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase));
			
			if(pack == null)
				throw new System.IO.FileNotFoundException("Package could not be found !", packageName);
			
			return new TinyMPK(pack.FullName).ToDictionary(k => k.Key, v => v.Value.Data);
		}
	}
}

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
			
			var cleaned = Regex.Replace(content, @";(.*)$", string.Empty, RegexOptions.Multiline);
			var matchescleanedup = Regex.Matches(cleaned, @"\[(?<index>.+?)\](?<params>[^\[]+)", RegexOptions.Multiline).OfType<Match>();
			
			
			return Regex.Matches(Regex.Replace(content, @";(.*)$", string.Empty, RegexOptions.Multiline), @"\[(?<index>.+?)\](?<params>[^\[]+)", RegexOptions.Multiline).OfType<Match>()
				.Select(region =>
				        new KeyValuePair<string, IDictionary<string, string>>(region.Groups["index"].Value.Trim().ToLower(),
				                                                              Regex.Matches(region.Groups["params"].Value, @"(?<name>[a-z0-9'_\-\. ]+)=(?<value>[a-z0-9'_\-\. ]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).OfType<Match>()
				                                                              .Select(sub =>
				                                                                      new KeyValuePair<string, string>(sub.Groups["name"].Value.Trim().ToLower(),
				                                                                                                       sub.Groups["value"].Value.Trim()))
				                                                              .GroupBy(kv => kv.Key).Select(grp => grp.First()).ToDictionary(k => k.Key, v => v.Value)
				                                                             )
				       ).GroupBy(kv => kv.Key).Select(grp => grp.First()).ToDictionary(k => k.Key, v => v.Value);
		}
		
		/// <summary>
		/// Extract a File from MPK package 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="packageName"></param>
		/// <param name="fileName"></param>
		/// <returns>File Byte Array</returns>
		public static byte[] GetFileDataFromPackage(this FileInfo[] data, string packageName, string fileName)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (string.IsNullOrEmpty(packageName))
				throw new ArgumentNullException("packageName");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			
			var pack = data.FirstOrDefault(file => file.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase));
			
			if (pack == null)
				throw new FileNotFoundException("Package could not be found !", packageName);
			
			var result = new TinyMPK(pack)[fileName];
			
			if (result == null)
				throw new FileNotFoundException(string.Format("File could not be found in package {0} !", packageName), fileName);
			
			return result.Data;
		}
		
		/// <summary>
		/// Extract a File from given MPK package 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fileName"></param>
		/// <returns>File Byte Array</returns>
		public static byte[] GetFileDataFromPackage(this FileInfo data, string fileName)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
						
			if (!data.Exists)
				throw new FileNotFoundException("Package could not be found !", data.FullName);
			
			var result = new TinyMPK(data)[fileName];
			
			if (result == null)
				throw new FileNotFoundException(string.Format("File could not be found in package {0} !", data.FullName), fileName);
			
			return result.Data;
		}
		
		/// <summary>
		/// Extract a File from MPK package 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="packageName"></param>
		/// <returns>File Byte Array</returns>
		public static IDictionary<string, byte[]> GetAllFileDataFromPackage(this FileInfo[] data, string packageName)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (string.IsNullOrEmpty(packageName))
				throw new ArgumentNullException("packageName");
			
			var pack = data.FirstOrDefault(file => file.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase));
			
			if(pack == null)
				throw new FileNotFoundException("Package could not be found !", packageName);
			
			return new TinyMPK(pack).ToDictionary(k => k.Key, v => v.Value.Data);
		}
	}
}

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

namespace DaocClientLib.MPK
{
	/// <summary>
	/// Represents a file stored in an MPK archive.
	/// </summary>
	public class TinyMPKFile
	{
		/// <summary>
		/// Byte From Extracted File
		/// </summary>
		private readonly byte[] _buf;
		
		/// <summary>
		/// Gets the unencrypted Data in the MPK
		/// </summary>
		public byte[] Data { get { return _buf; } }
		
		/// <summary>
		/// In-Archive File Name
		/// </summary>
		public string Name { get; protected set; }
		
		/// <summary>
		/// Constructs a new MPK file entry
		/// </summary>
		/// <param name="data">The uncompressed data of this file entry</param>
		/// <param name="filename">The file name</param>
		public TinyMPKFile(string filename, byte[] data)
		{
			Name = filename;
			_buf = data;
		}
	}
}

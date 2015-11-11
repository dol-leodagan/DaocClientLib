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

namespace DaocClientLib.Drawing
{
	using System;
	using System.IO;
	
	/// <summary>
	/// ClientDrawingWrapper is a ClientDataWrapper sub class implementing Drawing Primitives
	/// </summary>
	public class ClientDrawingWrapper : ClientDataWrapper
	{
		/// <summary>
		/// Retrieve Indexed Zone Renderer
		/// </summary>
		public virtual ZoneRendererChooser ZonesRenderer { get { return new ZoneRendererChooser(this); } }
		
		/// <summary>
		/// Create a Client Drawing Wrapper from Directory Path
		/// </summary>
		/// <param name="path"></param>
		public ClientDrawingWrapper(string path)
			: this(new DirectoryInfo(path))
		{
		}

		/// <summary>
		/// Create a Client Drawing Wrapper from Directory Path and given Regex File Filters
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filters"></param>
		public ClientDrawingWrapper(string path, string[] filters)
			: this(new DirectoryInfo(path), filters)
		{
		}
		
		/// <summary>
		/// Create a Client Drawing Wrapper from Directory Path
		/// </summary>
		/// <param name="path"></param>
		public ClientDrawingWrapper(DirectoryInfo path)
			: this(path, null)
		{
		}
		
		/// <summary>
		/// Create a Client Drawing Wrapper from Directory Path and given Regex File Filters
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filters"></param>
		public ClientDrawingWrapper(DirectoryInfo path, string[] filters)
			: base(path, filters)
		{
		}
	}
}

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

namespace DaocClientLib
{
	/// <summary>
	/// TreeData is an override map for client Tree's Position and Model
	/// </summary>
	public sealed class TreeData
	{
		/// <summary>
		/// Tree Offset on the X axis
		/// </summary>
		public float OffsetX { get; private set; }
		/// <summary>
		/// Tree Offset on the Y axis (Map Axis not 3D Axis !)
		/// </summary>
		public float OffsetY { get; private set; }
		/// <summary>
		/// Tree Offset on the Z axis (Map Height not 3D Axis !)
		/// </summary>
		public float OffsetZ { get; private set; }
		
		/// <summary>
		/// Nif Replacement Name
		/// </summary>
		public string RealNif { get; private set; }
		/// <summary>
		/// Replacement SPT file
		/// </summary>
		public string Replacement { get; private set; }
		/// <summary>
		/// Tree Bark Texture
		/// </summary>
		public string BarkTexture { get; private set; }
		/// <summary>
		/// Tree Leaf Texture
		/// </summary>
		public string LeafTexture { get; private set; }
		
		/// <summary>
		/// Create from Treemap Data (Replacement doesn't seem to be used)
		/// </summary>
		/// <param name="nif"></param>
		/// <param name="replacement"></param>
		/// <param name="barkTex"></param>
		/// <param name="leafTex"></param>
		/// <param name="zOffset"></param>
		public TreeData(string nif, string replacement, string barkTex, string leafTex, short zOffset)
			: this()
		{
			RealNif = nif;
			OffsetZ = (float)zOffset;
			
			Replacement = replacement;
			BarkTexture = barkTex;
			LeafTexture = leafTex;
		}

		/// <summary>
		/// Create from Tree Cluster Data
		/// </summary>
		/// <param name="nif"></param>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		/// <param name="zOffset"></param>
		/// <param name="replacement"></param>
		/// <param name="barkTex"></param>
		/// <param name="leafTex"></param>
		public TreeData(string nif, float xOffset, float yOffset, float zOffset, string replacement, string barkTex, string leafTex)
			: this()
		{
			RealNif = nif;
			OffsetX = xOffset;
			OffsetY = yOffset;
			OffsetZ = zOffset;
			
			Replacement = replacement;
			BarkTexture = barkTex;
			LeafTexture = leafTex;
		}
		
		/// <summary>
		/// Default Value initialization
		/// </summary>
		private TreeData()
		{
			OffsetX = 0;
			OffsetY = 0;
			OffsetZ = 0;
		}
	}
}

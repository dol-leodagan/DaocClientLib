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
	using System.Collections.Generic;
	using System.Linq;
	
	using Niflib;
	using Niflib.Extensions;
	
	#if OpenTK
	using OpenTK;
	using Matrix = OpenTK.Matrix4;
	#elif SharpDX
	using SharpDX;
	#elif MonoGame
	using Microsoft.Xna.Framework;
	#endif

	
	/// <summary>
	/// ZoneRenderer Build primitive 3D object From Zone Geometry Data
	/// </summary>
	public class ZoneRenderer : ZoneGeometry
	{
		/// <summary>
		/// Client Data Wrapper for Accessing Geometry Data
		/// </summary>
		protected ClientDataWrapper ClientWrapper { get; set; }
		
		/// <summary>
		/// Nif Primitives Cache
		/// </summary>
		public IDictionary<int, ClientMesh> NifCache { get; protected set; }
				
		/// <summary>
		/// Nif Instances Matrix
		/// Index Nif Cache Meshes with a World Instance Matrix
		/// </summary>
		public KeyValuePair<int, Matrix>[] InstancesMatrix { get; protected set; }
		
		/// <summary>
		/// TreeReplacement Map Cache
		/// </summary>
		protected TreeReplacementMap TreeReplacement { get; set; }
		
		/// <summary>
		/// Matrix To rotate Meshes 
		/// </summary>
		protected static Matrix RotationMatrix = Matrix.CreateRotationX((float)(Math.PI / -2f));

		/// <summary>
		/// Layers Categories to Retrieve From Nif Objects
		/// </summary>
		protected virtual IDictionary<string, string[]> Layers
		{
			get 
			{
				return new Dictionary<string, string[]> {
					{ "pickee", new []{ "pickee", "collidee", "visible", "exterior", "near", } },
					{ "collidee", new []{ "collidee", "visible", "exterior", "near", } },
					{ "visible", new []{ "visible", "exterior", "near", } },
					{ "climb", new []{ "climb[0-9]{3}", } },
					{ "door", new []{ "door[0-9]{3}", } },
				};
			}
		}
		
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="id"></param>
		/// <param name="files"></param>
		/// <param name="type"></param>
		/// <param name="wrapper"></param>
		public ZoneRenderer(int id, IEnumerable<FileInfo> files, ZoneType type, ClientDataWrapper wrapper)
			: base(id, files, type)
		{
			TreeReplacement = wrapper.TreeReplacement;
			NifCache = new Dictionary<int, ClientMesh>();
			InstancesMatrix = new KeyValuePair<int, Matrix>[0];
			ClientWrapper = wrapper;			
		}
		
		/// <summary>
		/// Init Nif Cache with a collection of Nifs References
		/// </summary>
		/// <param name="nifs"></param>
		protected void AddNifCache(IDictionary<int, string> nifs)
		{
			// Store Nifs Meshes
			foreach(var nif in nifs)
				AddNifMesh(nif.Key, nif.Value);
		}
		
		/// <summary>
		/// Add Nif Reference in Mesh Cache
		/// </summary>
		/// <param name="id"></param>
		/// <param name="nif"></param>
		protected void AddNifMesh(int id, string nif)
		{
			var trees = TreeReplacement[nif];
			
			var nifName = nif;
			// Tree match, replace nif name
			if (trees.Length > 0)
			{
				nifName = trees.First().RealNif;
			}
			
			// Store Layered Nif in Cache
			var meshes = GetNifMeshFromName(nif);
			NifCache.Add(id, meshes);
		}
		
		/// <summary>
		/// Search Nif File Name through all Client Files and extract Primitives Layers needed for displaying
		/// </summary>
		/// <param name="nifname"></param>
		/// <returns></returns>
		protected ClientMesh GetNifMeshFromName(string nifname)
		{
			// Get File Bytes
			var bytes = ClientWrapper.SearchRawFileOrPackaged(nifname, new []{ ".npk", ".mpk" });
			
			if (bytes != null)
			{
				return new ClientMesh(nifname, bytes);
			}
			
			return null;
		}
	}
}

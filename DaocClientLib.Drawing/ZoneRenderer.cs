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
		protected IDictionary<int, IDictionary<string, TriangleCollection>> NifCache { get; set; }
		
		/// <summary>
		/// Layers Categories to Retrieve From Nif Objects
		/// </summary>
		protected virtual IDictionary<string, string[]> Layers
		{
			get 
			{
				return new Dictionary<string, string[]>() {
					{ "pickee", new []{ "pickee", "collidee", "visible", "exterior", } },
					{ "collidee", new []{ "collidee", "visible", "exterior", } },
					{ "visible", new []{ "visible", "exterior", } },
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
			ClientWrapper = wrapper;
		}
		
		/// <summary>
		/// Add Nif Reference in Mesh Cache
		/// </summary>
		/// <param name="id"></param>
		/// <param name="nif"></param>
		protected void AddNifMesh(int id, string nif)
		{
			var trees = ClientWrapper.TreeReplacement[nif];
			
			var nifName = nif;
			// Tree match, replace nif name
			if (trees.Length > 0)
			{
				nifName = trees.First().RealNif;
			}
			
			// Store Layered Nif in Cache
			NifCache.Add(id, GetNifMeshesFromName(nif));
		}
		
		/// <summary>
		/// Search Nif File Name through all Client Files and extract Primitives Layers needed for displaying
		/// </summary>
		/// <param name="nifname"></param>
		/// <returns></returns>
		protected IDictionary<string, TriangleCollection> GetNifMeshesFromName(string nifname)
		{
			var result = new Dictionary<string, TriangleCollection>();
			// Get File Bytes
			var bytes = ClientWrapper.SearchRawFileOrPackaged(nifname, new []{ ".npk", ".mpk" });
			
			if (bytes != null)
			{
				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						var nif = new NiFile(reader);
						// Extract each Layer Type
						foreach (var layers in Layers)
						{
							TriangleCollection tris = new TriangleCollection { Vertices = new Vector3[0], Indices = new TriangleIndex[0] };
							// Try to find the requested Layer or iterate for alternate layer
							foreach (var layer in layers.Value)
							{
								var nifLayer = nif.GetTriangleFromCategories(layer);
								// if we have any result, concat triangle collection and break from loop
								if (nifLayer.Count > 0)
								{
									tris = nifLayer.Select(kv => kv.Value).Aggregate(new TriangleCollection { Vertices = new Vector3[0], Indices = new TriangleIndex[0] },
									                                                 (t1, t2) =>
									                                                 {
									                                                 	TriangleCollection concat;
									                                                 	TriangleWalker.Concat(ref t1, ref t2, out concat);
									                                                 	return concat;
									                                                 });
									break;
								}
							}
							
							// Concat Triangle to Existing Nif Layers or add to result
							if (tris.Indices.Length > 0)
							{
								TriangleCollection existing;
								if (result.TryGetValue(layers.Key, out existing))
								{
									TriangleCollection concat;
									TriangleWalker.Concat(ref existing, ref tris, out concat);
									result[layers.Key] = concat;
								}
								else
								{
									result.Add(layers.Key, tris);
								}
							}
						}
					}
				}
			}
			
			return result;
		}
	}
}

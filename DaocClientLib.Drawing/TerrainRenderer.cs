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
	using System.Linq;
	using System.Collections.Generic;
	
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
	/// Zone Renderer For Terrain Type
	/// </summary>
	public class TerrainRenderer : ZoneRenderer
	{		
		
		/// <summary>
		/// TerrainHeightCalculatorCache
		/// </summary>
		protected TerrainHeightCalculator TerrainHeightCache { get; set; }

		public TerrainRenderer(int id, IEnumerable<FileInfo> files, ZoneType type, ClientDataWrapper wrapper)
			: base(id, files, type, wrapper)
		{
			// Init Terrain Height Calculator
			TerrainHeightCache = TerrainHeightCalculator;
			
			AddNifCache(TerrainNifs);
			AddNifInstancesYZSwapped(TerrainFixtures);
			
			// Store Terrain
			var terrain = TerrainHeightMap;
			var vertices = new List<Vector3>();
			var indices = new List<TriangleIndex>();
			for (int x = 0 ; x < terrain.Length ; x++)
			{
				var yLength = terrain[x].Length;
				for (int y = 0 ; y < yLength ; y++)
				{
					var height = terrain[x][y];
					vertices.Add(new Vector3(x, height, y));
				}
			}
			
			var width = terrain.Length;
			for (int x = 0 ; x < terrain.Length - 1 ; x++)
			{
				var yLength = terrain[x].Length;
				for (int y = 0 ; y < yLength - 1 ; y++)
				{
					var tri1 = new TriangleIndex
					{
						A = (uint)((x + 1) * width + (y + 1)),
						B = (uint)((x + 1) * width + y),
						C = (uint)(x * width + y),
					};
					var tri2 = new TriangleIndex
					{
						A = (uint)((x + 1) * width + (y + 1)),
						B = (uint)(x * width + y),
						C = (uint)(x * width + (y + 1)),
					};
					
					indices.Add(tri1);
					indices.Add(tri2);
				}
			}
			
			// Build Terrain object as a Primitive Nif
			var Terrain = new TriangleCollection
			{
				Vertices = vertices.ToArray(),
				Indices = indices.ToArray(),
			};
			var TerrainNormals = Terrain.ComputeNormalLighting();
			
			var dictMesh = new Dictionary<string, TriangleCollection>();
			var dictNorms = new Dictionary<string, Vector3[]>();
			foreach (var layer in new []{ "pickee", "collidee", "visible", })
			{
				var layername = layer;
				dictMesh.Add(layername, Terrain);
				dictNorms.Add(layername, TerrainNormals);
			}
			
			// Add Terrain like a Nif
			var insertid = 0;
			if (NifCache.Count > 0)
				insertid = NifCache.Max(kv => kv.Key) + 1;
			
			NifCache.Add(insertid, dictMesh);
			NifNormalsCache.Add(insertid, dictNorms);
			InstancesMatrix = InstancesMatrix.Concat(new [] { new KeyValuePair<int, Matrix>(insertid, Matrix.Identity) }).ToArray();
		}
		
		/// <summary>
		/// Add Each Nif Geometry as an Instanced Meshes with World Matrix computed
		/// </summary>
		/// <param name="geometries">Collection of Nif Geometry</param>
		protected void AddNifInstancesYZSwapped(IEnumerable<NifGeometry> geometries)
		{
			var result = new List<KeyValuePair<int, Matrix>>();
			foreach (var geometry in geometries)
			{
				var trees = TreeReplacement[geometry.FileName];
				var nifMatrix = ComputeWorldMatrix(geometry, UnitFactor, TerrainHeightCache);
				// Tree match, Compose Matrix
				if (trees.Length > 0)
				{
					foreach (var tree in trees)
					{
						Matrix instance;
						Matrix translate;
						ZoneDrawingExtensions.CreateTranslation(tree.OffsetX * UnitFactor, tree.OffsetZ * UnitFactor, tree.OffsetY * UnitFactor, out instance);
						ZoneDrawingExtensions.Mult(ref nifMatrix, ref instance, out translate);
						nifMatrix = translate;
					}
				}
								
				result.Add(new KeyValuePair<int, Matrix>(geometry.MeshID, nifMatrix));
			}
			
			InstancesMatrix = InstancesMatrix.Concat(result).ToArray();
		}

		/// <summary>
		/// Get World Matrix For NifGeometry
		/// </summary>
		/// <param name="nifGeom"></param>
		/// <param name="UnitFactor"></param>
		/// <param name="heightCalculator"></param>
		/// <returns></returns>
		protected static Matrix ComputeWorldMatrix(NifGeometry nifGeom, float UnitFactor, TerrainHeightCalculator heightCalculator)
		{
			if (nifGeom == null)
				throw new ArgumentNullException("nifGeom");
			
			// Find Ground
			float nifHeight = nifGeom.Z * UnitFactor;
			if (nifGeom.OnGround)
			{
				nifHeight = heightCalculator[nifGeom.X, nifGeom.Y] * UnitFactor;
			}
			
			// Get Translation (XY Inverted), Scale, Rotation
			Matrix translation;
			ZoneDrawingExtensions.CreateTranslation(nifGeom.X * UnitFactor, nifHeight, nifGeom.Y * UnitFactor, out translation);
			Matrix scale;
			ZoneDrawingExtensions.CreateScale(nifGeom.Scale * UnitFactor, out scale);
			Matrix rotation;
			ZoneDrawingExtensions.CreateRotation(new Vector3(nifGeom.RotationX, nifGeom.RotationY, nifGeom.RotationZ), nifGeom.Angle , out rotation);
			
			Matrix result = Matrix.Identity;
			// Flip if needed
			if (nifGeom.Flip)
			{
				Matrix flip;
				ZoneDrawingExtensions.CreateScale(-1f, 1f, 1f, out flip);
				result = flip;
			}
			
			Matrix intermediateResult;
			// Combine Scale, Rotation, Invertion Rotation, Translation
			ZoneDrawingExtensions.Mult(ref result, ref scale, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref rotation, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref RotationMatrix, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref translation, out intermediateResult);
			result = intermediateResult;

			// Combine Matrix with Parent Matrix
			if (nifGeom.RelativeTo != null)
			{
				Matrix relativeMatrix = ComputeWorldMatrix(nifGeom.RelativeTo, UnitFactor, heightCalculator);
				ZoneDrawingExtensions.Mult(ref result, ref relativeMatrix, out intermediateResult);
				result = intermediateResult;
			}
			
			return result;
		}
	}
}

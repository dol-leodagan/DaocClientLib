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
	using System.Linq;
	using System.IO;
	using System.Collections.Generic;

	#if OpenTK
	using OpenTK;
	using Matrix = OpenTK.Matrix4;
	#elif SharpDX
	using SharpDX;
	#elif MonoGame
	using Microsoft.Xna.Framework;
	#endif

	/// <summary>
	/// Zone Renderer for Dungeon Type
	/// </summary>
	public class DungeonRenderer : ZoneRenderer
	{
		public DungeonRenderer(int id, IEnumerable<FileInfo> files, ZoneType type, ClientDataWrapper wrapper)
			: base(id, files, type, wrapper)
		{
			AddNifCache(DungeonChunk.Select((s, i) => new KeyValuePair<int, string>(i, s)).ToDictionary(v => v.Key, v => v.Value));
			AddNifInstancesYZSwapped(DungeonPlaces);
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
				var nifMatrix = ComputeWorldMatrix(geometry, UnitFactor);
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
		/// <returns></returns>
		protected static Matrix ComputeWorldMatrix(NifGeometry nifGeom, float UnitFactor)
		{
			if (nifGeom == null)
				throw new ArgumentNullException("nifGeom");
			
			// Get Translation (XY Inverted), Scale, Rotation
			Matrix translation;
			ZoneDrawingExtensions.CreateTranslation(nifGeom.X * UnitFactor, nifGeom.Y * UnitFactor, nifGeom.Z * UnitFactor, out translation);
			Matrix scale;
			ZoneDrawingExtensions.CreateScale(nifGeom.Scale * UnitFactor, out scale);
			Matrix rotation;
			ZoneDrawingExtensions.CreateRotation(new Vector3(nifGeom.RotationX, nifGeom.RotationY, nifGeom.RotationZ * -1f), nifGeom.Angle , out rotation);
			
			Matrix result = Matrix.Identity;
			// Flip if needed
			if (nifGeom.Flip)
			{
				Matrix flip;
				ZoneDrawingExtensions.CreateScale(-1f, 1f, 1f, out flip);
				result = flip;
			}
			
			Matrix intermediateResult;
			// Combine Scale, Rotation, Translation, Invertion Rotation
			ZoneDrawingExtensions.Mult(ref result, ref scale, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref rotation, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref translation, out intermediateResult);
			result = intermediateResult;
			ZoneDrawingExtensions.Mult(ref result, ref RotationMatrix, out intermediateResult);
			result = intermediateResult;

			// Combine Matrix with Parent Matrix
			if (nifGeom.RelativeTo != null)
			{
				Matrix relativeMatrix = ComputeWorldMatrix(nifGeom.RelativeTo, UnitFactor);
				ZoneDrawingExtensions.Mult(ref result, ref relativeMatrix, out intermediateResult);
				result = intermediateResult;
			}
			
			return result;
		}
	}
}

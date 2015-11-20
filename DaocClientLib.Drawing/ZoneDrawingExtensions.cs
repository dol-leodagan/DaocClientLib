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
	
	#if OpenTK
	using OpenTK;
	using Matrix = OpenTK.Matrix4;
	#elif SharpDX
	using SharpDX;
	#elif MonoGame
	using Microsoft.Xna.Framework;
	#endif
	using Niflib;
	using Niflib.Extensions;
	
	/// <summary>
	/// ZoneDrawingExtensions Contains Method for Generating 3D Drawing Collections from Client Lib Zone Files
	/// </summary>
	public static class ZoneDrawingExtensions
	{
		/// <summary>
		/// Get World Matrix For NifGeometry
		/// </summary>
		/// <param name="nifGeom"></param>
		/// <param name="translateFactor"></param>
		/// <param name="heightCalculator"></param>
		/// <returns></returns>
		public static Matrix ComputeWorldMatrix(this NifGeometry nifGeom, float translateFactor, TerrainHeightCalculator heightCalculator)
		{
			if (nifGeom == null)
				throw new ArgumentNullException("nifGeom");
			
			// Find Ground
			float nifHeight = nifGeom.Z * translateFactor;
			if (nifGeom.OnGround)
			{
				nifHeight = heightCalculator[nifGeom.X, nifGeom.Y] * translateFactor;
			}
			
			// Start with Indentity Matrix
			Matrix result = Matrix.Identity;
			
			// Get Translation, Scale, Rotatio
			Matrix translation;
			CreateTranslation(nifGeom.X * translateFactor, nifGeom.Y * translateFactor, nifHeight, out translation);
			Matrix scale;
			CreateScale(nifGeom.Scale * translateFactor, out scale);
			Matrix rotation;
			CreateRotation(new Vector3(nifGeom.RotationX, nifGeom.RotationY, nifGeom.RotationZ), nifGeom.Angle , out rotation);
			
			Matrix intermediateResult;
			// Flip
			if (nifGeom.Flip)
			{
				Matrix flip;
				CreateScale(-1f, 1f, 1f, out flip);
				Mult(ref result, ref flip, out intermediateResult);
				result = intermediateResult;
			}
			// Combine Translation, Scale, Rotation
			Mult(ref result, ref scale, out intermediateResult);
			result = intermediateResult;
			Mult(ref result, ref rotation, out intermediateResult);
			result = intermediateResult;
			Mult(ref result, ref translation, out intermediateResult);
			result = intermediateResult;

			// Combine Matrix with Parent Matrix
			if (nifGeom.RelativeTo != null)
			{
				Matrix relativeMatrix = nifGeom.RelativeTo.ComputeWorldMatrix(translateFactor, heightCalculator);
				Mult(ref result, ref relativeMatrix, out intermediateResult);
				result = intermediateResult;
			}
			
			return result;
		}
		
		/// <summary>
		/// Get Translation Matrix for TreeData
		/// </summary>
		/// <param name="tree"></param>
		/// <returns></returns>
		public static Matrix ComputeWorldMatrix(this TreeData tree, float translateFactor)
		{
			Matrix translation;
			CreateTranslation(tree.OffsetX * translateFactor, tree.OffsetY * translateFactor, tree.OffsetZ * translateFactor, out translation);
			return translation;
		}
		
		/// <summary>
		/// Matrix Overload for Framework Match
		/// </summary>
		public static void Mult(ref Matrix left, ref Matrix right, out Matrix result)
		{
			#if OpenTK
			Matrix.Mult(ref left, ref right, out result);
			#else
			Matrix.Multiply(ref left, ref right, out result);
			#endif
		}		
		/// <summary>
		/// Matrix Overload for Framework Match
		/// </summary>
		public static void CreateTranslation(float x, float y, float z, out Matrix result)
		{
			#if SharpDX
			Matrix.Translation(x, y, z, out result);
			#else
			Matrix.CreateTranslation(x, y, z, out result);
			#endif
		}
		/// <summary>
		/// Matrix Overload for Framework Match
		/// </summary>
		public static void CreateScale(float scale, out Matrix result)
		{
			#if SharpDX
			Matrix.Scaling(scale, out result);
			#else
			Matrix.CreateScale(scale, out result);
			#endif
		}
		/// <summary>
		/// Matrix Overload for Framework Match
		/// </summary>
		public static void CreateScale(float x, float y, float z, out Matrix result)
		{
			#if SharpDX
			Matrix.Scaling(x, y, z, out result);
			#else
			Matrix.CreateScale(x, y, z, out result);
			#endif
		}
		/// <summary>
		/// Matrix Overload for Framework Match
		/// </summary>
		public static void CreateRotation(Vector3 axis, float angle, out Matrix result)
		{
			#if SharpDX
			Matrix.RotationAxis(ref axis, angle, out result);
			#elif OpenTK
			Matrix.CreateFromAxisAngle(axis, angle, out result);
			#else
			Matrix.CreateFromAxisAngle(ref axis, angle, out result);
			#endif
		}
	}
}
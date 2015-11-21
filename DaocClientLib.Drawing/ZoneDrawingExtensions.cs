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
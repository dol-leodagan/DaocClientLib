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
	/// TerrainHeightCalculator allows to get the Terrain Height from X, Y coordinate
	/// </summary>
	public sealed class TerrainHeightCalculator
	{
		private readonly float[][] Heightmap;
		private readonly float Unit;
		
		/// <summary>
		/// Fast Vector3 Implementation out of Any Framework Requirement
		/// </summary>
		private struct Vector3
		{
			public float X;
			public float Y;
			public float Z;
			
			public Vector3(float x, float y, float z)
			{
				X = x; Y = y; Z = z;
			}
		}
		
		public float this[float X, float Y]
		{
			get
			{
				var x = X * Unit;
				var y = Y * Unit;
				var heightmapX = (int)Math.Floor(x);
				var heightmapY = (int)Math.Floor(y);
				
				Vector3 A;
				Vector3 B;
				Vector3 C;
				
				// Decide Which Triangle the Target is in
				if (x - heightmapX < y - heightmapY)
				{
					A = new Vector3(heightmapX+1, Heightmap[heightmapX+1][heightmapY+1], heightmapY+1);
					B = new Vector3(heightmapX+1, Heightmap[heightmapX+1][heightmapY], heightmapY);
					C = new Vector3(heightmapX, Heightmap[heightmapX][heightmapY], heightmapY);
				}
				else
				{
					A = new Vector3(heightmapX+1, Heightmap[heightmapX+1][heightmapY+1], heightmapY+1);
					B = new Vector3(heightmapX, Heightmap[heightmapX][heightmapY], heightmapY);
					C = new Vector3(heightmapX, Heightmap[heightmapX][heightmapY+1], heightmapY+1);
				}
				
				var result = calcY(ref A, ref B, ref C, x, y) / Unit;
				return result;
			}
		}
		
		/// <summary>
		/// Calc Y From Triangle of Vector3 given X and Z
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="x"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		static float calcY(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, float x, float z) {
			float det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
			
			float l1 = ((p2.Z - p3.Z) * (x - p3.X) + (p3.X - p2.X) * (z - p3.Z)) / det;
			float l2 = ((p3.Z - p1.Z) * (x - p3.X) + (p1.X - p3.X) * (z - p3.Z)) / det;
			float l3 = 1.0f - l1 - l2;
			
			return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
		}

		public TerrainHeightCalculator(float[][] heightmap, float unit)
		{
			Unit = unit;
			Heightmap = heightmap;
		}
	}
}

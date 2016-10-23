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
	/// Dungeon Water Geometry.
	/// </summary>
	public sealed class DungeonWaterGeometry
	{
		/// <summary>
		/// Dungeon Water Geometry Arbitrary ID
		/// </summary>
		public int ID { get; private set; }
		
		/// <summary>
		/// Dungeon Water X1
		/// </summary>
		public float X1 { get; private set; }
		/// <summary>
		/// Dungeon Water Y1
		/// </summary>
		public float Y1 { get; private set; }
		/// <summary>
		/// Dungeon Water Z1
		/// </summary>
		public float Z1 { get; private set; }
		
		/// <summary>
		/// Dungeon Water X2
		/// </summary>
		public float X2 { get; private set; }
		/// <summary>
		/// Dungeon Water Y2
		/// </summary>
		public float Y2 { get; private set; }
		/// <summary>
		/// Dungeon Water Z2
		/// </summary>
		public float Z2 { get; private set; }
		
		/// <summary>
		/// Dungeon Water X3
		/// </summary>
		public float X3 { get; private set; }
		/// <summary>
		/// Dungeon Water Y3
		/// </summary>
		public float Y3 { get; private set; }
		/// <summary>
		/// Dungeon Water Z3
		/// </summary>
		public float Z3 { get; private set; }
		
		/// <summary>
		/// Dungeon Water X4
		/// </summary>
		public float X4 { get; private set; }
		/// <summary>
		/// Dungeon Water Y4
		/// </summary>
		public float Y4 { get; private set; }
		/// <summary>
		/// Dungeon Water Z4
		/// </summary>
		public float Z4 { get; private set; }
		
		/// <summary>
		/// Dungeon Water TranslationX
		/// </summary>
		public float TranslationX { get; private set; }
		/// <summary>
		/// Dungeon Water TranslationY
		/// </summary>
		public float TranslationY { get; private set; }
		/// <summary>
		/// Dungeon Water TranslationZ
		/// </summary>
		public float TranslationZ { get; private set; }
		
		/// <summary>
		/// Create new instance of <see cref="DungeonWaterGeometry"/>
		/// </summary>
		public DungeonWaterGeometry(int ID, float X1, float Y1, float Z1,
		                            float X2, float Y2, float Z2,
		                            float X3, float Y3, float Z3,
		                            float X4, float Y4, float Z4,
		                            float TranslationX, float TranslationY, float TranslationZ)
		{
			this.ID = ID;
			this.X1 = X1;
			this.Y1 = Y1;
			this.Z1 = Z1;
			this.X2 = X2;
			this.Y2 = Y2;
			this.Z2 = Z2;
			this.X3 = X3;
			this.Y3 = Y3;
			this.Z3 = Z3;
			this.X4 = X4;
			this.Y4 = Y4;
			this.Z4 = Z4;
			this.TranslationX = TranslationX;
			this.TranslationY = TranslationY;
			this.TranslationZ = TranslationZ;
		}
	}
}

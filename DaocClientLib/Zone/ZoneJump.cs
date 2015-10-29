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
	/// ZoneJump Describe "Zoning" Jump point Geometry
	/// </summary>
	public sealed class ZoneJump
	{
		/// <summary>
		/// Zone Jump Client ID
		/// </summary>
		public short ID { get; private set; }
		/// <summary>
		/// Zone Jump In-file ID
		/// </summary>
		public short Order { get; private set; }
		/// <summary>
		/// Zone Jump Name
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Plane X1
		/// </summary>
		public int X1 { get; private set; }
		/// <summary>
		/// Plane Y1
		/// </summary>
		public int Y1 { get; private set; }
		/// <summary>
		/// Plane X2
		/// </summary>
		public int X2 { get; private set; }
		/// <summary>
		/// Plane Y2
		/// </summary>
		public int Y2 { get; private set; }
		/// <summary>
		/// Plane Z1
		/// </summary>
		public int Z1 { get; private set; }
		/// <summary>
		/// Plane Z2
		/// </summary>
		public int Z2 { get; private set; }
		
		/// <summary>
		/// Default Zone Jump Constructor 
		/// </summary>
		public ZoneJump(short id, short order, string name, int x1, int y1, int x2, int y2, int z1, int z2)
		{
			ID = id;
			Order = order;
			Name = name;
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
			Z1 = z1;
			Z2 = z2;
		}
	}
}

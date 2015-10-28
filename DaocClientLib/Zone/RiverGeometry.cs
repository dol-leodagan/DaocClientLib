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
using System.Collections.Generic;
using System.Linq;

namespace DaocClientLib
{
	/// <summary>
	/// RiverGeometry Store Data about Zone Rivers
	/// </summary>
	public sealed class RiverGeometry
	{
		/// <summary>
		/// Collection of River Banks
		/// </summary>
		public RiverBank[] Banks { get; private set; }
		
		/// <summary>
		/// River ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		/// River Name
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// River Type (RIVER/LAVA)
		/// </summary>
		public string Type { get; private set; }
		/// <summary>
		/// River Texture
		/// </summary>
		public string Texture { get; private set; }
		/// <summary>
		/// River Multitexture
		/// </summary>
		public string Multitexture { get; private set; }
		/// <summary>
		/// River Flow
		/// </summary>
		public short Flow { get; private set; }
		/// <summary>
		/// River Height
		/// </summary>
		public int Height { get; private set; }
		/// <summary>
		/// River Color
		/// </summary>
		public int Color { get; private set; }
		/// <summary>
		/// River Extend Pos X
		/// </summary>
		public int ExtendPosX { get; private set; }
		/// <summary>
		/// River Extend Pos Y
		/// </summary>
		public int ExtendPosY { get; private set; }
		/// <summary>
		/// River Extend Neg X
		/// </summary>
		public int ExtendNegX { get; private set; }
		/// <summary>
		/// River Extend Neg Y
		/// </summary>
		public int ExtendNegY { get; private set; }
		/// <summary>
		/// River Tesselation
		/// </summary>
		public short Tesselation { get; private set; }
		
		
		/// <summary>
		/// Load River from Values
		/// </summary>
		public RiverGeometry(int id, string name, string type, string texture, string multitexture, short flow, int height, int color, int extend_posx, int extend_posy,
		                     int extend_negx, int extend_negy, short tesselation, IEnumerable<Tuple<IEnumerable<short>, IEnumerable<short>>> banks)
		{
			ID = id;
			Name = name;
			Type = type;
			Texture = texture;
			Multitexture = multitexture;
			Flow = flow;
			Height = height;
			Color = color;
			ExtendPosX = extend_posx;
			ExtendPosY = extend_posy;
			ExtendNegX = extend_negx;
			ExtendNegY = extend_negy;
			Tesselation = tesselation;
			Banks = banks.Select(t => new RiverBank(t.Item1, t.Item2)).ToArray();
		}
		
		/// <summary>
		/// RiverBank Sub Class to store Geometry of RiverBanks
		/// </summary>
		public sealed class RiverBank
		{
			/// <summary>
			/// Left Bank X
			/// </summary>
			public short LeftX { get; private set; }
			/// <summary>
			/// Left Bank Y
			/// </summary>
			public short LeftY { get; private set; }
			/// <summary>
			/// Left Bank Z
			/// </summary>
			public short LeftZ { get; private set; }

			/// <summary>
			/// Right Bank X
			/// </summary>
			public short RightX { get; private set; }
			/// <summary>
			/// Right Bank Y
			/// </summary>
			public short RightY { get; private set; }
			/// <summary>
			/// Right Bank Z
			/// </summary>
			public short RightZ { get; private set; }
			
			/// <summary>
			/// Load RiverBank From Enumerable of Left and Right Coordinate
			/// </summary>
			/// <param name="lefts"></param>
			/// <param name="rights"></param>
			public RiverBank(IEnumerable<short> lefts, IEnumerable<short> rights)
			{
				LeftX = lefts.ElementAt(0);
				LeftY = lefts.ElementAt(1);
				LeftZ = lefts.ElementAt(2);
				RightX = rights.ElementAt(0);
				RightY = rights.ElementAt(1);
				RightZ = rights.ElementAt(2);
			}
		}
	}
}

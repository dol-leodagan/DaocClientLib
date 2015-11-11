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
	
	/// <summary>
	/// ZoneRendererChooser indexes all Zones Renderer Data
	/// </summary>
	public class ZoneRendererChooser : ZoneGeometryChooser
	{
		/// <summary>
		/// Client Data Wrapper for Accessing Geometry Data
		/// </summary>
		protected ClientDataWrapper ClientWrapper { get; set; }

		public new ZoneRenderer this[int Index]
		{
			get
			{
				FileInfo[] files;
				ZoneType type;
				if (m_zoneTypeDict.TryGetValue(Index, out type))
				{
					if (m_zoneDict.TryGetValue(Index, out files))
					{
						switch(type)
						{
							default:
							case ZoneType.Terrain:
								return new TerrainRenderer(Index, files, type, ClientWrapper);
							case ZoneType.City:
								return new CityRenderer(Index, files, type, ClientWrapper);
							case ZoneType.Dungeon:
							case ZoneType.InstancedDungeon:
								return new DungeonRenderer(Index, files, type, ClientWrapper);
								
								
						}
					}
				}
				
				return null;
			}
		}
		public ZoneRendererChooser(ClientDataWrapper client)
			: base(client)
		{
			ClientWrapper = client;
		}
	}
}

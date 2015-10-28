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
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace DaocClientLib
{
	/// <summary>
	/// ZoneGeometry parse Client Geometry Data
	/// </summary>
	public class ZoneGeometry
	{
		/// <summary>
		/// Terrain Max Integer Unit
		/// </summary>
		public const byte TerrainFactor = 255;
		/// <summary>
		/// Game Unit to 3D Scape Factor
		/// </summary>
		public const float UnitFactor = TerrainFactor/65535f;

		/// <summary>
		/// This Zone Files
		/// </summary>
		private readonly FileInfo[] m_files;
		/// <summary>
		/// This Zone DAT Package
		/// </summary>
		private string DatPackage { get { return string.Format("dat{0}", ID.ToString("000")); } }
		/// <summary>
		/// This Zone Sector.dat File Name
		/// </summary>
		private string SectorFile { get { return "sector.dat"; } }
		/// <summary>
		/// This Zone Terrain File Name
		/// </summary>
		private string TerrainFile { get { return "terrain.pcx"; } }
		/// <summary>
		/// This Zone Offset File Name
		/// </summary>
		private string OffsetFile { get { return "offset.pcx"; } }
		/// <summary>
		/// This Zone Offset File Name
		/// </summary>
		private string WaterFile { get { return "water.pcx"; } }
		
		/// <summary>
		/// This Zone ID
		/// </summary>
		public int ID { get; protected set; }
		
		/// <summary>
		/// This Zone Terrain Scale
		/// </summary>
		public short TerrainScale { get; protected set; }
		/// <summary>
		/// This Zone Terrain Offset
		/// </summary>
		public short TerrainOffset { get; protected set; }
		
		/// <summary>
		/// This Zone Sector X Size
		/// </summary>
		public short SizeX { get; protected set; }
		/// <summary>
		/// This Zone Sector Y Size
		/// </summary>
		public short SizeY { get; protected set; }
		
		/// <summary>
		/// This Zone Rivers
		/// </summary>
		public RiverGeometry[] Rivers { get; protected set; }
		
		#region getter
		/// <summary>
		/// Get Terrain Height Map Indexed By Pixel X*Y
		/// </summary>
		public float[][] TerrainHeightMap
		{
			get
			{
				var terrainImg = m_files.GetFileDataFromPackage(DatPackage, TerrainFile);
				var offsetImg = m_files.GetFileDataFromPackage(DatPackage, OffsetFile);
				
				var terrainPcx = new PCXDecoder(terrainImg).PcxImage;
				var offsetPcx = new PCXDecoder(offsetImg).PcxImage;
				
				var map = new float[terrainPcx.Width][];
				
				for (int x = 0 ; x < terrainPcx.Width ; x++)
				{
					map[x] = new float[terrainPcx.Height];
					for (int y = 0 ; y < terrainPcx.Height ; y++)
					{
						var terrainVal = terrainPcx.GetPixel(x, y).R / (float)TerrainFactor * (float)TerrainScale;
						var offsetVal = offsetPcx.GetPixel(x, y).R / (float)TerrainFactor * (float)TerrainOffset;
						map[x][y] = terrainVal + offsetVal;
					}
				}
				
				return map;
			}
		}
		
		/// <summary>
		/// Get Terrain Water Index Map, Only existing Water defs are presents in the Collection
		/// </summary>
		public IDictionary<int, IDictionary<int, RiverGeometry>> WaterIndexMap
		{
			get
			{
				var waterImg = m_files.GetFileDataFromPackage(DatPackage, WaterFile);
				var waterPcx = new PCXDecoder(waterImg).PcxImage;
				var result = new Dictionary<int, IDictionary<int, RiverGeometry>>();
				for (int x = 0 ; x < waterPcx.Width ; x++)
				{
					for (int y = 0 ; y < waterPcx.Height ; y++)
					{
						var grey = waterPcx.GetPixel(x, y).R;
						var river = grey < Rivers.Length ? Rivers.FirstOrDefault(r => r.ID == grey) : null;
						
						if (river != null)
						{
							if (!result.ContainsKey(x))
								result[x] = new Dictionary<int, RiverGeometry>();

							result[x][y] = river;
						}
					}
				}
				
				return result;
			}
		}
		#endregion
		
		public ZoneGeometry(int id, IEnumerable<FileInfo> files)
		{
			ID = id;
			m_files = files.ToArray();
			IDictionary<string, IDictionary<string, string>> sectorDat;
			try
			{
				sectorDat = m_files.GetFileDataFromPackage(DatPackage, SectorFile).ReadDATFile();
			}
			catch (Exception e)
			{
				throw new ArgumentException(string.Format("No sector.dat Found when building Zone ID: {0}", id), "files", e);
			}
			
			// Read Terrain
			IDictionary<string, string> terrain;
			if (sectorDat.TryGetValue("terrain", out terrain))
			{
				string scalefactor;
				string offsetfactor;
				TerrainScale = terrain.TryGetValue("scalefactor", out scalefactor) ? short.Parse(scalefactor) : (short)-1;
				TerrainOffset = terrain.TryGetValue("offsetfactor", out offsetfactor) ? short.Parse(offsetfactor) : (short)-1;
			}
			else
			{
				TerrainScale = -1;
				TerrainOffset = -1;
			}
			// Read Sector Size			
			IDictionary<string, string> sectorsize;
			if (sectorDat.TryGetValue("sectorsize", out sectorsize))
			{
				string sizex;
				string sizey;
				SizeX = sectorsize.TryGetValue("sizex", out sizex) ? short.Parse(sizex) : (short)-1;
				SizeY = sectorsize.TryGetValue("sizey", out sizey) ? short.Parse(sizey) : (short)-1;
			}
			else
			{
				SizeX = -1;
				SizeY = -1;
			}
			// Read Water Definitions
			IDictionary<string, string> waterdefs;
			if (sectorDat.TryGetValue("waterdefs", out waterdefs))
			{
				string num;
				var rivers = new List<RiverGeometry>();
				if (waterdefs.TryGetValue("num", out num))
				{
					var numWater = short.Parse(num);
					for (int w = 0 ; w < numWater ; w++)
					{
						IDictionary<string, string> river;
						if (sectorDat.TryGetValue(string.Format("river{0}", w.ToString("00")), out river))
						{
							string texture;
							string multitexture;
							string flow;
							string height;
							string bankpoints;
							string color;
							string extend_posx;
							string extend_posy;
							string extend_negx;
							string extend_negy;
							string tesselation;
							string name;
							string type;
							
							short r_flow = river.TryGetValue("flow", out flow) ? short.Parse(flow) : (short)0;
							int r_height = river.TryGetValue("height", out height) ? int.Parse(height) : -1;
							short r_bankpoints = river.TryGetValue("bankpoints", out bankpoints) ? short.Parse(bankpoints) : (short)0;
							int r_color = river.TryGetValue("color", out color) ? int.Parse(color) : 0;
							int r_extend_posx = river.TryGetValue("extend_posx", out extend_posx) ? int.Parse(extend_posx) : 0;
							int r_extend_posy = river.TryGetValue("extend_posy", out extend_posy) ? int.Parse(extend_posy) : 0;
							int r_extend_negx = river.TryGetValue("extend_negx", out extend_negx) ? int.Parse(extend_negx) : 0;
							int r_extend_negy = river.TryGetValue("extend_negy", out extend_negy) ? int.Parse(extend_negy) : 0;
							short r_tesselation = river.TryGetValue("tesselation", out tesselation) ? short.Parse(tesselation) : (short)0;
							
							river.TryGetValue("texture", out texture);
							river.TryGetValue("multitexture", out multitexture);
							river.TryGetValue("name", out name);
							river.TryGetValue("type", out type);
							
							var banks = new List<Tuple<IEnumerable<short>, IEnumerable<short>>>();
							for (int b = 0 ; b < r_bankpoints ; b++)
							{
								string left;
								string right;
								if (!river.TryGetValue(string.Format("left{0}", b.ToString("00")), out left))
									continue;
								if (!river.TryGetValue(string.Format("right{0}", b.ToString("00")), out right))
									continue;
								
								banks.Add(new Tuple<IEnumerable<short>, IEnumerable<short>>(left.Split(',').Select(s => short.Parse(s)), right.Split(',').Select(s => short.Parse(s))));
							}
							
							var riverGeo = new RiverGeometry(w, name, type, texture, multitexture, r_flow, r_height, r_color, r_extend_posx, r_extend_posy, r_extend_negx, r_extend_negy, r_tesselation, banks);
							rivers.Add(riverGeo);
						}
					}
				}
				Rivers = rivers.ToArray();
			}
			else
			{
				Rivers = new RiverGeometry[0];
			}
		}
	}
}

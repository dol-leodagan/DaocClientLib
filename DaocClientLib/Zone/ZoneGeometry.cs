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
using System.Globalization;

namespace DaocClientLib
{
	/// <summary>
	/// Zone Type Enum
	/// </summary>
	public enum ZoneType
	{
		Terrain = 0,
		Dungeon = 2,
		City = 1,
		InstancedDungeon = 4,
	}
	
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
		public const float UnitFactor = TerrainFactor/65536f;

		/// <summary>
		/// This Zone Files
		/// </summary>
		private readonly FileInfo[] m_files;
		/// <summary>
		/// This Zone DAT Package
		/// </summary>
		private string DatPackage { get { return string.Format("dat{0}.mpk", ID.ToString("000")); } }
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
		/// This Zone JumpPoint File Name
		/// </summary>
		private string ZoneJumpFile { get { return "zonejump.csv"; } }		
		/// <summary>
		/// This Zone JumpPoint File Name
		/// </summary>
		private string BoundFile { get { return "bound.csv"; } }
		/// <summary>
		/// This Zone Dungeon Chunk File
		/// </summary>
		private string DungeonChunkFile { get { return "dungeon.chunk"; } }
		/// <summary>
		/// This Zone Dungeon Place File
		/// </summary>
		private string DungeonPlaceFile { get { return "dungeon.place"; } }
		/// <summary>
		/// This Zone Dungeon Props File
		/// </summary>
		private string DungeonPropFile { get { return "dungeon.place"; } }
		/// <summary>
		/// This Zone Terrain Nifs File
		/// </summary>
		private string TerrainNifFile { get { return "nifs.csv"; } }
		/// <summary>
		/// This Zone Terrain Fixtures File
		/// </summary>
		private string TerrainFixtureFile { get { return "fixtures.csv"; } }
		/// <summary>
		/// This Zone City File
		/// </summary>
		private string CityFile { get { return "city.csv"; } }
		
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
		
		#region data getter
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
		/// Get Heigh Calculator
		/// </summary>
		public TerrainHeightCalculator TerrainHeightCalculator
		{
			get
			{					
				var terrain = TerrainHeightMap;
				return new TerrainHeightCalculator(terrain, UnitFactor);
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
		
		/// <summary>
		/// Get Zone Jump Point List
		/// </summary>
		public IDictionary<int, ZoneJump> ZoneJumps
		{
			get
			{
				var zonejumpfile = m_files.GetFileDataFromPackage(DatPackage, ZoneJumpFile);
				var result = new Dictionary<int, ZoneJump>();
				foreach(var row in zonejumpfile.ReadCSVFile())
				{
					if (row.Length < 9)
						continue;
					
					short id = short.Parse(row[8]);
					short order = short.Parse(row[0]);
					string name = row[1];
					int x1 = int.Parse(row[2]);
					int y1 = int.Parse(row[3]);
					int x2 = int.Parse(row[4]);
					int y2 = int.Parse(row[5]);
					int z1 = int.Parse(row[6]);
					int z2 = int.Parse(row[7]);
					
					var jump = new ZoneJump(id, order, name, x1, y1, x2, y2, z1, z2);
					
					result.Add(id, jump);
				}
				
				return result;
			}
		}
		
		/// <summary>
		/// Get Zone Bounds Coordinates, Shape from X/Y line points
		/// </summary>
		public Tuple<int, int>[][] Bounds
		{
			get
			{
				var zoneboundfile = m_files.GetFileDataFromPackage(DatPackage, BoundFile);
				var result = new List<Tuple<int, int>[]>();
				foreach (var row in zoneboundfile.ReadCSVFile())
				{
					if (row.Length < 2)
						continue;
					
					int count = Math.Min(row.Length - 2, int.Parse(row[1]) * 2) + 2;
					var subresult = new List<Tuple<int, int>>();
					for (int i = 2 ; i < count ; i=i+2)
					{
						subresult.Add(new Tuple<int, int>(int.Parse(row[i]), int.Parse(row[i+1])));
					}
					
					result.Add(subresult.ToArray());
				}
				
				return result.ToArray();
			}
		}
		
		/// <summary>
		/// Get Dungeon Chunk, Nif file String Index
		/// </summary>
		public string[] DungeonChunk
		{
			get
			{
				return m_files.GetFileDataFromPackage(DatPackage, DungeonChunkFile).ReadCSVFile().Select(line => line.FirstOrDefault()).ToArray();
			}
		}
		
		/// <summary>
		/// Get Dungeon Places, Nif File Collection with Transformation Geometry
		/// </summary>
		public NifGeometry[] DungeonPlaces
		{
			get
			{
				var chunks = DungeonChunk;
				int index = -1;
				return m_files.GetFileDataFromPackage(DatPackage, DungeonPlaceFile).ReadCSVFile()
					.Select(line =>
					        {
					        	if (line.Length < 8)
					        		return null;

					        	index++;					        	
					        	// Chunk Nif Reference
					        	int nifId = int.Parse(line.First());
					        	string nifName = chunks[nifId];
					        	
					        	float X = float.Parse(line[1], CultureInfo.InvariantCulture);
					        	float Y = float.Parse(line[2], CultureInfo.InvariantCulture);
					        	float Z = float.Parse(line[3], CultureInfo.InvariantCulture);
					        	
					        	float Angle = float.Parse(line[4], CultureInfo.InvariantCulture);
					        	float RotX = float.Parse(line[5], CultureInfo.InvariantCulture);
					        	float RotY = float.Parse(line[6], CultureInfo.InvariantCulture);
					        	float RotZ = float.Parse(line[7], CultureInfo.InvariantCulture);
					        	
					        	bool Flip = false;
					        	if (line.Length > 10) // FIXME: is this really Flip ?
					        		Flip = short.Parse(line[10]) != 0;
					        	
					        	return new NifGeometry(nifId, index, nifName, nifName, X, Y, Z, 1f, Angle, RotX, RotY, RotZ, Flip, false, null);
					        }
					       ).Where(nif => nif != null).ToArray();
			}
		}
		
		/// <summary>
		/// Get Dungeon Props, Nif File Collection With Transformation relative to Dungeon Places
		/// </summary>
		public NifGeometry[] DungeonProps
		{
			get
			{
				var chunks = DungeonChunk;
				var places = DungeonPlaces;
				int index = -1;
				return m_files.GetFileDataFromPackage(DatPackage, DungeonPropFile).ReadCSVFile()
					.Select(line =>
					        {
					        	if (line.Length < 11)
					        		return null;
					        	
					        	index++;					        	
					        	// Chunk nif Reference
					        	int nifId = int.Parse(line.First());
					        	string nifName = chunks[nifId];
					        	
					        	// Place nif Reference
					        	int placeId = int.Parse(line[9]);
					        	var place = places.FirstOrDefault(nif => nif.ID == placeId);
					        	
					        	float X = float.Parse(line[1], CultureInfo.InvariantCulture);
					        	float Y = float.Parse(line[2], CultureInfo.InvariantCulture);
					        	float Z = float.Parse(line[3], CultureInfo.InvariantCulture);
					        	
					        	float Angle = float.Parse(line[4], CultureInfo.InvariantCulture);
					        	float RotX = float.Parse(line[5], CultureInfo.InvariantCulture);
					        	float RotY = float.Parse(line[6], CultureInfo.InvariantCulture);
					        	float RotZ = float.Parse(line[7], CultureInfo.InvariantCulture);
					        	
					        	float Scale = float.Parse(line[10]);
					        	
					        	return new NifGeometry(nifId, index, nifName, nifName, X, Y, Z, Scale, Angle, RotX, RotY, RotZ, false, false, place);
					        }).Where(nif => nif != null).ToArray();
			}
		}
		
		/// <summary>
		/// Get Terrain Nifs ID to Nif File Dictionary
		/// </summary>
		public IDictionary<int, string> TerrainNifs
		{
			get
			{
				return m_files.GetFileDataFromPackage(DatPackage, TerrainNifFile).ReadCSVFile()
					.Select(line =>
					        {
					        	if (line.Length > 0 && (line[0].StartsWith("Grid Nifs", StringComparison.OrdinalIgnoreCase)
					        	                        || line[0].StartsWith("NIF", StringComparison.OrdinalIgnoreCase))
					        	                       || line.Length < 3)
					        		return new KeyValuePair<int, string>(-1, null);
					        	
					        	return new KeyValuePair<int, string>(int.Parse(line[0]), line[2]);
					        }).Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
			}
		}
		
		/// <summary>
		/// Get Terrain Fixture with Nifs From Terrain Nifs
		/// </summary>
		public NifGeometry[] TerrainFixtures
		{
			get
			{
				var nifs = TerrainNifs;
				int index = -1;
				return m_files.GetFileDataFromPackage(DatPackage, TerrainFixtureFile).ReadCSVFile()
					.Select(line =>
					        {
					        	if (line.Length < 13 || (line.Length > 0 && (line[0].StartsWith("Fixtures", StringComparison.OrdinalIgnoreCase)
					        	                                             || line[0].StartsWith("ID", StringComparison.OrdinalIgnoreCase))))
					        		return null;
					        	
					        	index++;
					        	string nifname;
					        	int nifId = int.Parse(line[1]);
					        	if (!nifs.TryGetValue(nifId, out nifname))
					        		return null;
					        	
					        	float X = float.Parse(line[3], CultureInfo.InvariantCulture);
					        	float Y = float.Parse(line[4], CultureInfo.InvariantCulture);
					        	float Z = float.Parse(line[5], CultureInfo.InvariantCulture);
					        	int A = int.Parse(line[6]);
					        	float Scale = int.Parse(line[7]) / 100f;
					        	bool ground = int.Parse(line[11]) > 0;
					        	bool flip = int.Parse(line[12]) > 0; // FIXME : is this really flip ?
					        	float Angle = 0f;
					        	float RotX = 0f;
					        	float RotY = 0f;
					        	float RotZ = 0f;
					        	if (A > 180)
					        	{
					        		Angle = (float)((A - 360) * Math.PI / 180f * -1f);
					        		RotZ = 1f;
					        	}
					        	else
					        	{
					        		Angle  = (float)(A * Math.PI / 180f);
					        		RotZ = -1f;
					        	}
					        	// Long Version
					        	if (line.Length > 18)
					        	{
						        	Angle = float.Parse(line[15], CultureInfo.InvariantCulture);
						        	RotX = float.Parse(line[16], CultureInfo.InvariantCulture);
						        	RotY = float.Parse(line[17], CultureInfo.InvariantCulture);
						        	RotZ = float.Parse(line[18], CultureInfo.InvariantCulture);
					        	}
					        	// TODO angle / A ?? Z ??
					        	return new NifGeometry(nifId, index, nifname, line[2], X, Y, Z, Scale, Angle, RotX, RotY, RotZ, flip, ground, null);
					        }).Where(nif => nif != null).ToArray();
			}
		}
		
		/// <summary>
		/// Get City Nifs Files Indexed by ID
		/// </summary>
		public IDictionary<int, string> CityNifs
		{
			get
			{
				return m_files.GetFileDataFromPackage(DatPackage, CityFile).ReadCSVFile()
					.Where(line => line.Length > 1 && !string.IsNullOrEmpty(line[1])).Select(line => new KeyValuePair<int, string>(int.Parse(line[0]), line[1]))
					.ToDictionary(kv => kv.Key, kv => kv.Value);
			}
		}
		#endregion
		
		
		#region type getter
		/// <summary>
		/// This Zone Detected Type
		/// </summary>
		public ZoneType ZoneType { get; protected set; }
		
		/// <summary>
		/// Is this Zone a Terrain ?
		/// </summary>
		public bool IsTerrain
		{
			get { return ZoneType == ZoneType.Terrain; }
		}
		/// <summary>
		/// Is this Zone a Dungeon ?
		/// </summary>
		public bool IsDungeon
		{
			get { return ZoneType == ZoneType.Dungeon || ZoneType == ZoneType.InstancedDungeon; }
		}
		/// <summary>
		/// Is this Zone a City ?
		/// </summary>
		public bool IsCity
		{
			get { return ZoneType == ZoneType.City; }
		}
		#endregion
		/// <summary>
		/// Default Construtor Intialize Values with Sector.Dat
		/// </summary>
		/// <param name="id"></param>
		/// <param name="files"></param>
		public ZoneGeometry(int id, IEnumerable<FileInfo> files, ZoneType type)
		{
			if (files == null)
				throw new ArgumentNullException("files");
			
			ID = id;
			m_files = files.ToArray();
			IDictionary<string, IDictionary<string, string>> sectorDat;
			try
			{
				sectorDat = m_files.GetFileDataFromPackage(DatPackage, SectorFile).ReadDATFile();
			}
			catch (Exception e)
			{
				throw new ArgumentException(string.Format("No usable sector.dat Found when building Zone ID: {0}", id), "files", e);
			}
			
			// Assign Zone Type
			ZoneType = type;
			
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
							string rtype;
							
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
							river.TryGetValue("type", out rtype);
							
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
							
							var riverGeo = new RiverGeometry(w, name, rtype, texture, multitexture, r_flow, r_height, r_color, r_extend_posx, r_extend_posy, r_extend_negx, r_extend_negy, r_tesselation, banks);
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

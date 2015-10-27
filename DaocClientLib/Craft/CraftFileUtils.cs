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
using System.IO;

namespace DaocClientLib
{
	/// <summary>
	/// Structure of Binary Craft List Header
	/// </summary>
	class CraftHeader
	{
		public uint Version;
		public uint NameListSize;
		public uint NumberOfNames;
		public uint NameListOffset;
		
		// 3 of them
		public CraftRealmData[] RealmData;		
		public const int RealmCount = 3;
		
		public class CraftRealmData
		{
			public uint NumberOfRecipes;
			public uint NumberOfCategories;
			public uint RecipeListOffset;
			public uint CategoryListOffset;
			public uint ProfessionListOffset;
		}
	}
	
	/// <summary>
	/// Structure of Binary Craft List Item
	/// </summary>
	class CraftItem
	{
		public uint NameIndex;
		public uint BaseMaterial;
		public uint Id;
		public ushort Pic;
		public ushort Skill;
		public ushort MaterialLevel;
		public ushort Level;
		
		// 8 of them
		public CraftMaterials[] Materials;		
		public const int MaterialsCount = 8;
		
		public class CraftMaterials
		{
			public uint NameIndex;
			public ushort Count;
			public ushort BaseMaterial;
		}
	}
	
	/// <summary>
	/// Structure of Binary Craft List Profession Data
	/// </summary>
	class CraftProfessionData
	{
		//201 empty bytes...
		public ushort[] Empty;
		public const int EmptyBytes = 201;
		//19 Categories...
		public CraftProfession[] Professions;
		public const int CategoriesCount = 19;
				
		/// <summary>
		/// Structure of Binary Craft List Profession
		/// </summary>
		public class CraftProfession
		{
			// 3 debug bytes ??
			public ushort[] Debug;
			public const int DebugBytes = 3;
			public ushort NameIndex;
			// 200 Indexes ?
			public ushort[] Indexes;			
			public const int IndexesCount = 200;
		}
	}
	
	/// <summary>
	/// Structure of Binary Craft List Category
	/// </summary>
	class CraftCategory
	{
		public uint NameIndex;
		// 50 Recipe ID ?
		public ushort[] RecipeIds;		
		public const int RecipesCount = 50;
	}

	/// <summary>
	/// Extension Class for reading Binary Stream into Structure
	/// </summary>
	static class CraftFileReader
	{
		public static CraftHeader ReadHeader(this Stream fs)
		{
			var result = new CraftHeader();
			result.RealmData = new CraftHeader.CraftRealmData[CraftHeader.RealmCount];
			
			result.Version = fs.ReadUint();
			result.NameListSize = fs.ReadUint();
			result.NumberOfNames = fs.ReadUint();
			result.NameListOffset = fs.ReadUint();
			
			for (int c = 0 ; c < CraftHeader.RealmCount ; c++)
			{
				var realm = new CraftHeader.CraftRealmData();
				realm.NumberOfRecipes = fs.ReadUint();
				realm.NumberOfCategories = fs.ReadUint();
				realm.RecipeListOffset = fs.ReadUint();
				realm.CategoryListOffset = fs.ReadUint();
				realm.ProfessionListOffset = fs.ReadUint();
				result.RealmData[c] = realm;
			}
			
			return result;
		}

		public static string[] ReadStrings(this Stream fs, uint size, uint count)
		{
			var result = new string[count];
			
			var index = 0;
			var start = fs.Position;
			var end = start + size;
			
			fs.Position = start;
			
			while(index < count && fs.Position < end)
			{
				result[index] = fs.ReadCString();
				index++;
			}
			
			return result;
		}

		public static CraftProfessionData ReadProfessions(this Stream fs)
		{
			var result = new CraftProfessionData();
						
			result.Empty = new ushort[CraftProfessionData.EmptyBytes];
			
			for (int e = 0 ; e < CraftProfessionData.EmptyBytes ; e++)
				result.Empty[e] = fs.ReadUshort();
			
			result.Professions = new CraftProfessionData.CraftProfession[CraftProfessionData.CategoriesCount];
			
			for (int c = 0 ; c < CraftProfessionData.CategoriesCount ; c++)
			{
				var prof = new CraftProfessionData.CraftProfession();
				prof.Debug = new ushort[CraftProfessionData.CraftProfession.DebugBytes];
				
				for (int d = 0 ; d < CraftProfessionData.CraftProfession.DebugBytes ; d++)
					prof.Debug[d] = fs.ReadUshort();
				
				prof.NameIndex = fs.ReadUshort();
				prof.Indexes = new ushort[CraftProfessionData.CraftProfession.IndexesCount];
				
				for (int i = 0 ; i < CraftProfessionData.CraftProfession.IndexesCount ; i++)
					prof.Indexes[i] = fs.ReadUshort();
				
				result.Professions[c] = prof;
			}
			
			return result;
		}
		
		public static CraftItem[] ReadRecipes(this Stream fs, uint count)
		{
			var result = new CraftItem[count];
			
			for (int r = 0 ; r < count ; r++)
			{
				var current = new CraftItem();
				
				current.NameIndex = fs.ReadUint();
				current.BaseMaterial = fs.ReadUint();
				current.Id = fs.ReadUint();
				current.Pic = fs.ReadUshort();
				current.Skill = fs.ReadUshort();
				current.MaterialLevel = fs.ReadUshort();
				current.Level = fs.ReadUshort();
				
				current.Materials = new CraftItem.CraftMaterials[CraftItem.MaterialsCount];
				
				for (int m = 0 ; m < CraftItem.MaterialsCount ; m++)
				{
					var mat = new CraftItem.CraftMaterials();
					mat.NameIndex = fs.ReadUint();
					mat.Count = fs.ReadUshort();
					mat.BaseMaterial = fs.ReadUshort();
					current.Materials[m] = mat;
				}
				
				result[r] = current;
			}
			
			return result;
		}
		
		public static CraftCategory[] ReadCategories(this Stream fs, uint count)
		{
			var result = new CraftCategory[count];
			
			for (int c = 0 ; c < count ; c++)
			{
				var current = new CraftCategory();
				current.NameIndex = fs.ReadUint();
				current.RecipeIds = new ushort[CraftCategory.RecipesCount];
				
				for (int i = 0 ; i < CraftCategory.RecipesCount ; i++)
					current.RecipeIds[i] = fs.ReadUshort();
				
				result[c] = current;
			}
			
			return result;
		}
		
		public static uint ReadUint(this Stream fs)
		{
			var buff = new byte[4];
			
			for (int i = 0 ; i < 4 ; i++)
				buff[i] = (byte)fs.ReadByte();
			
			return BitConverter.ToUInt32(buff, 0);
		}
		
		public static ushort ReadUshort(this Stream fs)
		{
			var buff = new byte[2];
			
			for (int i = 0 ; i < 2 ; i++)
				buff[i] = (byte)fs.ReadByte();
			
			return BitConverter.ToUInt16(buff, 0);
		}
		
		public static string ReadCString(this Stream fs)
		{
			var buff = new List<byte>();
			int current;
			while(true)
			{
				current = fs.ReadByte();
				
				if (current != 0 && current != -1)
					buff.Add((byte)current);
				else
					break;
			}
			
			return System.Text.Encoding.Default.GetString(buff.ToArray());
		}
	}
}

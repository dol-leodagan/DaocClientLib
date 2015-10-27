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

namespace DaocClientLib
{
	/// <summary>
	/// Craft Recipe
	/// </summary>
	public class CraftDataRecipe
	{
		/// <summary>
		/// Recipe ID
		/// </summary>
		public int ID;
		/// <summary>
		/// Recipe Name
		/// </summary>
		public string Name;
		/// <summary>
		/// Recipe Base Material
		/// </summary>
		public string BaseMaterialName;
		/// <summary>
		/// Recipe Originating Realm
		/// </summary>
		public string Realm;
		/// <summary>
		/// Recipe Needed Profession
		/// </summary>
		public string Profession;
		/// <summary>
		/// Recipe Needed SkillLevel
		/// </summary>
		public int SkillLevel;
		/// <summary>
		/// Recipe Resulting Level
		/// </summary>
		public int ItemLevel;
		/// <summary>
		/// Recipe Material Level
		/// </summary>
		public int MaterialLevel;
		/// <summary>
		/// Collections of Craft Data Ingredients Needed for Building
		/// </summary>
		public IEnumerable<CraftDataIngredient> Ingredients;
		
		/// <summary>
		/// Recipe Full Name with Base Material
		/// </summary>
		public string FullName
		{
			get
			{
				return string.IsNullOrEmpty(BaseMaterialName) ?
					Name :
					string.Format("{0} {1}", BaseMaterialName, Name);
			}
		}
		
		/// <summary>
		/// Ingredients string representions tab separated
		/// </summary>
		public string IngredientsString
		{
			get
			{
				var result = "";
				foreach(var ingredient in Ingredients)
				{
					result = string.Format("{0}{1}\t\t", result, ingredient);
				}
				return result;
			}
		}
		
		public override string ToString()
		{
			return string.Format("{0}\t{1}\tSkill {2}\t'{6}'\t {3} (Level {4}) => {5}", Realm, Profession, SkillLevel, FullName, ItemLevel, IngredientsString, ID);
		} 
	}
}

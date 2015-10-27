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
	/// Craft Recipe's Ingredient
	/// </summary>
	public class CraftDataIngredient
	{
		/// <summary>
		/// Ingredient Name
		/// </summary>
		public string Name;
		/// <summary>
		/// Ingredient Base Material
		/// </summary>
		public string BaseMaterialName;
		/// <summary>
		/// Ingredient Amount
		/// </summary>
		public int Count;
		/// <summary>
		/// Ingredient Sub Recipe Match if Any
		/// </summary>
		public CraftDataRecipe SubRecipe;
		
		/// <summary>
		/// Get Ingredient FullName with BaseMaterial
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
		/// Get Ingredient Full Name with Amount
		/// </summary>
		public string FullNameCount
		{
			get
			{
				return string.Format("{0} {1}", Count, FullName);
			}
		}
		public override string ToString()
		{
			return SubRecipe == null ? FullNameCount : string.Format("{0} (Recipe {1})", FullNameCount, SubRecipe.ID);
		}
	}
}

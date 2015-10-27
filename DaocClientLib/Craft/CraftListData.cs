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
using System.IO;
using System.Collections.Generic;

namespace DaocClientLib
{
	/// <summary>
	/// CraftListData Read from Client Binary File and hold all Recipes Information for Query Use.
	/// </summary>
	public class CraftListData
	{
		/// <summary>
		/// Hardcoded Base Material Strings...
		/// </summary>
		public static readonly string[] BaseMaterials = { "","","","","","","","Bronze",
			"Iron","Mithril","Asterite","","","","Alloy","Steel",
			/*16*/"","Rawhide","Tanned","Cured","Hard","Rigid","Embossed","Imbued",
			"Runed","Eldritch","Fine Alloy","","","Adamantium","","Rowan",
			/*32*/"Elm","Oak","Ironwood","Heartwood","Runewood","Stonewood","Ebonwood","Dyrwood",
			"","Woolen","Linen","Brocade","Silk","Gossamer","Sylvan","Seamist",
			/*48*/"Nightshade","Wyvernskin","Leaf","Bone","Vine","Shell","Fossil","Amber",	
			"Coral","Chitin","Copper","Ferrite","Quartz","Dolomite","Cobalt","Carbide",
			/*64*/"Sapphire","Diamond","Netherite","Arcanite","Netherium","Arcanium","Tempered","Duskwood",
			"Silksteel","Petrified","Crystalized","","","","","",
			/*80*/"raw","uncut","rough","flawed","imperfect","polished","faceted","precious",
			"flawless","perfect","","","","","","" };
		
		/// <summary>
		/// Hardcoded Basic Crafting Name for Matching Recipe Copies
		/// </summary>
		public const string BasicCraftingName = "Basic Crafting";
		
		/// <summary>
		/// Collection of All Client Recipes
		/// </summary>
		public CraftDataRecipe[] Recipes;
		
		/// <summary>
		/// Create a Craft Recipes Collection from Binary Stream
		/// </summary>
		/// <param name="craftBinaryFile"></param>
		public CraftListData(Stream craftBinaryFile)
		{
			if (craftBinaryFile == null)
				throw new ArgumentNullException("craftBinaryFile");
			
			if (!craftBinaryFile.CanRead || !craftBinaryFile.CanSeek)
				throw new IOException("Can't Read or Seek Binary Craft File !");
			
			// Get File Header and String Content
			craftBinaryFile.Position = 0;
			var m_header = craftBinaryFile.ReadHeader();
			craftBinaryFile.Position += m_header.NameListOffset;
			var m_strings = craftBinaryFile.ReadStrings(m_header.NameListSize, m_header.NumberOfNames);
			
			// Initialize Realm Data
			var m_professionData = new CraftProfessionData[CraftHeader.RealmCount];
			var m_recipeList = new CraftItem[CraftHeader.RealmCount][];
			var m_categoryList = new CraftCategory[CraftHeader.RealmCount][];
			
			// For each Realm
			for (int r = 0 ; r < 3 ; r++)
			{
				var realmData = m_header.RealmData[r];
				
				// retrieve profession
				craftBinaryFile.Position = realmData.ProfessionListOffset;
				m_professionData[r] = craftBinaryFile.ReadProfessions();
				
				// retrieve recipe
				craftBinaryFile.Position = realmData.RecipeListOffset;
				m_recipeList[r] = craftBinaryFile.ReadRecipes(realmData.NumberOfRecipes);
				
				// retrieve categories
				craftBinaryFile.Position = realmData.CategoryListOffset;
				m_categoryList[r] = craftBinaryFile.ReadCategories(realmData.NumberOfCategories);
			}
			
			var recipesResult = new Dictionary<uint, List<CraftDataRecipe>>();
			
			// For each Realm
			for (uint r = 0 ; r < 3 ; r++)
			{
				string name;
				
				switch(r)
				{
					case 0:
						name = "Albion";
						break;
					case 1:
						name = "Midgard";
						break;
					default:
						name = "Hibernia";
						break;
				}
				
				if (!recipesResult.ContainsKey(r))
					recipesResult.Add(r, new List<CraftDataRecipe>());
				
				for (uint p = 0 ; p < m_professionData[r].Professions.Length ; p++)
				{
					var profNameIndex = m_professionData[r].Professions[p].NameIndex;
					if (profNameIndex == 0)
						continue;
					
					var ProfessionName = m_strings[profNameIndex];
					
					foreach (var c in m_professionData[r].Professions[p].Indexes.Where(i => i > 0))
					{
						var catNameIndex = m_categoryList[r][c].NameIndex;
						if (catNameIndex == 0)
							continue;
						
						var CategoryName = m_strings[catNameIndex];
						
						for (uint rc = 0 ; rc < m_categoryList[r][c].RecipeIds.Length ; rc++)
						{
							var RecipeIndex = m_categoryList[r][c].RecipeIds[rc];
							if (RecipeIndex == 0)
								continue;
							
							var recipe = m_recipeList[r][RecipeIndex];
							
							var RecipeNameIndex = recipe.NameIndex;
							
							if (RecipeNameIndex == 0)
								continue;
							
							if (recipe.Materials.All(mat => mat.Count <= 0))
								continue;
							
							//Create Recipe
							var Recipe = new CraftDataRecipe();
							Recipe.Name = m_strings[RecipeNameIndex];
							Recipe.ID = (int)recipe.Id;
							Recipe.ItemLevel = recipe.Level;
							Recipe.Profession = ProfessionName;
							Recipe.Realm = name;
							Recipe.SkillLevel = recipe.Skill;
							Recipe.MaterialLevel = recipe.MaterialLevel;
							Recipe.Ingredients = new List<CraftDataIngredient>();
							
							if (recipe.BaseMaterial != 0 && recipe.BaseMaterial < BaseMaterials.Length)
								Recipe.BaseMaterialName = BaseMaterials[recipe.BaseMaterial];
							
							for (uint i = 0 ; i < recipe.Materials.Length ; i++)
							{
								var ingredient = recipe.Materials[i];
								
								if (ingredient.Count <= 0 || ingredient.NameIndex == 0 || string.IsNullOrEmpty(m_strings[ingredient.NameIndex]))
									continue;
								
								var Ingredient = new CraftDataIngredient();
								Ingredient.Name = m_strings[ingredient.NameIndex];
								Ingredient.Count = ingredient.Count;
								if (ingredient.BaseMaterial != 0 && ingredient.BaseMaterial < BaseMaterials.Length)
									Ingredient.BaseMaterialName = BaseMaterials[ingredient.BaseMaterial];
								
								((List<CraftDataIngredient>)Recipe.Ingredients).Add(Ingredient);
							}
							
							recipesResult[r].Add(Recipe);
						}
					}
				}
			}
			
			var recipesLookup = new Dictionary<string, Dictionary<string, CraftDataRecipe>>();
			
			// Parsing Results
			foreach(var realmRecipe in recipesResult)
			{
				var eachRealm = realmRecipe.Key;
				
				foreach(var eachRecipe in realmRecipe.Value)
				{
					Dictionary<string, CraftDataRecipe> subrealm;
					if (!recipesLookup.TryGetValue(eachRecipe.Realm, out subrealm))
					{
						subrealm = new Dictionary<string, CraftDataRecipe>();
						recipesLookup.Add(eachRecipe.Realm, subrealm);
					}
					
					// Fix Basic Crafting Base Material Name by copying regular recipes
					if (eachRecipe.Profession == BasicCraftingName)
					{
						eachRecipe.BaseMaterialName = recipesResult[eachRealm].Where(rec => rec.Name == eachRecipe.Name && rec.MaterialLevel == eachRecipe.MaterialLevel).Select(rec => rec.BaseMaterialName).FirstOrDefault();
					}
					
					// Try Matching Sub-Recipe for some Ingredients
					foreach(var eachIngredient in eachRecipe.Ingredients)
					{
						CraftDataRecipe subrec;
						if (!subrealm.TryGetValue(eachIngredient.FullName, out subrec))
						{
							subrec = recipesResult[eachRealm].FirstOrDefault(rec => rec.Realm.Equals(eachRecipe.Realm) && rec.FullName.Equals(eachIngredient.FullName));
							subrealm.Add(eachIngredient.FullName, subrec);
						}
						
						eachIngredient.SubRecipe = subrec;
					}
				}
			}
			
			Recipes = recipesResult.SelectMany(kv => kv.Value).ToArray();
		}
		
		/// <summary>
		/// Returnes Craft Data	Recipe Colleciton From input File Byte Array
		/// </summary>
		/// <param name="input">file byte array</param>
		/// <returns></returns>
		public static CraftDataRecipe[] RecipesFromFileBytes(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			
			if (input.Length == 0)
				throw new ArgumentException("Input Array is Empty", "input");
			
			CraftDataRecipe[] results;
			
			using (var stream = new MemoryStream(input))
			{
				var craftList = new CraftListData(stream);
				results = craftList.Recipes;
			}
			
			return results;
		}
	}
	
}

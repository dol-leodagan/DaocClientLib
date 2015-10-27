﻿/*
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
	/// TreeReplacementMap is a class listing overrides for Tree Positions, every tree matching this data must obey the offsets for correct position
	/// </summary>
	public class TreeReplacementMap
	{
		/// <summary>
		/// Tree Map Data
		/// </summary>
		readonly Dictionary<string, TreeData> TreeMap;
		
		/// <summary>
		/// Tree Cluster Data Appended with Tree Map Data
		/// </summary>
		readonly Dictionary<string, IEnumerable<TreeData>> TreeCluster;
		
		/// <summary>
		/// Retrieve the given Tree Name Overrides
		/// </summary>
		public IEnumerable<TreeData> this[string treename]
		{
			get
			{
				var index = treename.ToLower();
				
				IEnumerable<TreeData> tc;
				
				if (TreeCluster.TryGetValue(index, out tc))
				{
					return tc;
				}
				
				TreeData tm;
				if (TreeMap.TryGetValue(index, out tm))
				{
					return new [] { tm };
				}
				
				return new [] { new TreeData(treename, "", "", "", 0) };
			}
		}
		
		/// <summary>
		/// Warnings during Parse
		/// </summary>
		protected List<Exception> m_Warnings = new List<Exception>();
		
		/// <summary>
		/// Get Warnings that occured during parse 
		/// </summary>
		public Exception[] Warnings { get { return m_Warnings.ToArray(); } }

		/// <summary>
		/// Build Tree Replacement Map from Tree Map and Tree Cluster CSV's
		/// </summary>
		/// <param name="treeMap"></param>
		/// <param name="treeCluster"></param>
		public TreeReplacementMap(IEnumerable<IEnumerable<string>> treeMap, IEnumerable<IEnumerable<string>> treeCluster)
		{
			// Create Single Tree Map
			TreeMap = treeMap.Where(l => l.Count() > 4)
				.ToDictionary(l => l.First().ToLower(), l => {
				              	try
				              	{
				              		return new TreeData(l.First(), l.ElementAt(1), l.ElementAt(2), l.ElementAt(3), short.Parse(l.ElementAt(4)));
				              	}
				              	catch (Exception e)
				              	{
				              		m_Warnings.Add(new NotSupportedException(string.Format("Could not parse Tree Map Data '{0}' from CSV", l.First().ToLower()), e));
				              		return null;
				              	}
				              });
			
			// Create Cluster Tree Map
			TreeCluster = new Dictionary<string, IEnumerable<TreeData>>();
			foreach(var cluster in treeCluster)
			{
				var count = cluster.Count();
				var list = new List<TreeData>();
				if (count > 1)
				{
					var originName = cluster.First().ToLower();
					if (originName.Equals("name", StringComparison.OrdinalIgnoreCase))
						continue;
					var targetName = cluster.Skip(1).First().ToLower();
					TreeData treeConstraint;
					var zOffset = 0f;
					var replacement = "";
					var barktex = "";
					var leaftex = "";
					if (TreeMap.TryGetValue(targetName, out treeConstraint))
					{
						zOffset = treeConstraint.OffsetZ;
						replacement = treeConstraint.Replacement;
						barktex = treeConstraint.BarkTexture;
						leaftex = treeConstraint.LeafTexture;
					}
					
					// get X, Y, Z for each instance
					for (int i = 2 ; i < count-2 ; i += 3)
					{
						try
						{
							var x = float.Parse(cluster.ElementAt(i));
							var y = float.Parse(cluster.ElementAt(i+1));
							var z = float.Parse(cluster.ElementAt(i+2));
							
							// empty data
							if (x == 0f && y == 0f && z == 0f)
								continue;
							
							list.Add(new TreeData(targetName, x, y, z+zOffset,
							                      replacement, barktex, leaftex));
						}
						catch (Exception e)
						{
							m_Warnings.Add(new NotSupportedException(string.Format("Could not parse Tree Cluster Data '{0}'(index {1} - {2}) from CSV", originName, i, i+2), e));
						}
					}
				
					TreeCluster.Add(originName, list.ToArray());
				}
			}
		}
	}
}

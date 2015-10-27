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

namespace DaocClientLib
{
	/// <summary>
	/// Zone Data Object
	/// </summary>
	public sealed class ZoneData
	{
		/// <summary>
		/// Zone Header Identifier
		/// </summary>
		public string ZoneHeader { get; private set; }
		/// <summary>
		/// Zone is Enabled
		/// </summary>
		public bool Enabled { get; private set; }
		/// <summary>
		/// Zone ID
		/// </summary>
		public short ID { get; private set; }
		/// <summary>
		/// Zone Type (Default -1)
		/// </summary>
		public short Type { get; private set; }
		/// <summary>
		/// Zone Region (Default -1)
		/// </summary>
		public short Region { get; private set; }
		/// <summary>
		/// Zone Name (default null)
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Zone OffsetX (Default -1)
		/// </summary>
		public short OffsetX { get; private set; }
		/// <summary>
		/// Zone OffsetY (Default -1)
		/// </summary>
		public short OffsetY { get; private set; }
		/// <summary>
		/// Zone Width (Default -1)
		/// </summary>
		public short Width { get; private set; }
		/// <summary>
		/// Zone Height (Default -1)
		/// </summary>
		public short Height { get; private set; }
		/// <summary>
		/// Zone Temperature (Default short.Minvalue)
		/// </summary>
		public short Temperature { get; private set; }
		/// <summary>
		/// Zone Entry Music ID (Default -1)
		/// </summary>
		public short EntryMusic { get; private set; }
		/// <summary>
		/// Zone SkyDome Descriptor (Default null)
		/// </summary>
		public string SkyDome { get; private set; }
		/// <summary>
		/// Zone Map is Enabled (Default true)
		/// </summary>
		public bool MapEnabled { get; private set; }
		/// <summary>
		/// Zone Proxy Zone (Default -1)
		/// </summary>
		public short ProxyZone { get; private set; }
		
		/// <summary>
		/// Zone Data Object From header String and Dictionary of Var=Value
		/// </summary>
		/// <param name="header"></param>
		/// <param name="content"></param>
		public ZoneData(string header, IDictionary<string, string> content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			if (string.IsNullOrEmpty(header))
				throw new ArgumentNullException("header");
			
			// Zone Identifiers
			ZoneHeader = header;
			ID = short.Parse(new string (header.Skip(4).ToArray()));
			// Defaults
			Type = -1;
			Region = -1;
			Name = null;
			OffsetX = -1;
			OffsetY = -1;
			Width = -1;
			Height = -1;
			Temperature = short.MinValue;
			EntryMusic = -1;
			SkyDome = null;
			MapEnabled = true;
			ProxyZone = -1;
			
			string enabled;
			if (content.TryGetValue("enabled", out enabled))
			{
				Enabled = int.Parse(enabled) > 0;
			}
			string type;
			if (content.TryGetValue("type", out type))
			{
				Type = short.Parse(type);
			}
			string region;
			if (content.TryGetValue("region", out region))
			{
				Region = short.Parse(region);
			}
			string name;
			if (content.TryGetValue("name", out name))
			{
				Name = name;
			}
			string region_offset_x;
			if (content.TryGetValue("region_offset_x", out region_offset_x))
			{
				OffsetX = short.Parse(region_offset_x);
			}
			string region_offset_y;
			if (content.TryGetValue("region_offset_y", out region_offset_y))
			{
				OffsetY = short.Parse(region_offset_y);
			}
			string width;
			if (content.TryGetValue("width", out width))
			{
				Width = short.Parse(width);
			}
			string height;
			if (content.TryGetValue("height", out height))
			{
				Height = short.Parse(height);
			}
			string temperature;
			if (content.TryGetValue("temperature", out temperature))
			{
				Temperature = short.Parse(temperature);
			}
			string entry_music;
			if (content.TryGetValue("entry_music", out entry_music))
			{
				EntryMusic = short.Parse(entry_music);
			}
			string skydome;
			if (content.TryGetValue("skydome", out skydome))
			{
				SkyDome = skydome;
			}
			string map_enabled;
			if (content.TryGetValue("map_enabled", out map_enabled))
			{
				MapEnabled = int.Parse(map_enabled) > 0;
			}
			string proxy_zone;
			if (content.TryGetValue("proxy_zone", out proxy_zone))
			{
				ProxyZone = short.Parse(proxy_zone);
			}
		}
	}
}

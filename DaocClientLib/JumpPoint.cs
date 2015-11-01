/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DaocClientLib
{
	/// <summary>
	/// JumpPoint Describe a Global /jump Position in Client Data.
	/// </summary>
	public sealed class JumpPoint
	{
		/// <summary>
		/// Jump Point ID, Arbitrary
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		/// Jump Point Name / Description
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Jump Point Zone ID
		/// </summary>
		public int ZoneID { get; private set; }
		/// <summary>
		/// Jump point X coordinate
		/// </summary>
		public int X { get; private set; }
		/// <summary>
		/// Jump point Y coordinate
		/// </summary>
		public int Y { get; private set; }
		/// <summary>
		/// Jump point Z coordinate
		/// </summary>
		public int Z { get; private set; }
		/// <summary>
		/// Jump point Heading in Degrees
		/// </summary>
		public short HeadingDegrees { get; private set; }
		/// <summary>
		/// Jump point also existing in Given Zone ID's (Mirrored Zone)
		/// </summary>
		public int[] AlsoInZoneID { get; private set; }
		
		/// <summary>
		/// Build a jump point from a CSV Parsed String Collection
		/// </summary>
		/// <param name="jumpPointLine"></param>
		public JumpPoint(IEnumerable<string> jumpPointLine)
		{
			var count = jumpPointLine.Count();
			
			if (count > 6)
			{
				ID = int.Parse(jumpPointLine[0]);
				Name = jumpPointLine[1];
				ZoneID = int.Parse(jumpPointLine[2]);
				X = int.Parse(jumpPointLine[3]);
				Y = int.Parse(jumpPointLine[4]);
				Z = int.Parse(jumpPointLine[5]);
				HeadingDegrees = short.Parse(jumpPointLine[6]);
				// Remaining ID's seems to be Mirror Zone, 0 can't be used as it looks like a "default" in the file...
				AlsoInZoneID = jumpPointLine.Skip().Where(s => !string.IsNullOrEmpty(s)).Select(s => int.Parse(s)).Where(id => id > 0).ToArray();
			}
			else
			{
				throw new NotSupportedException(string.Format("JumpPoint's Line doesn't have enough field to be parsed : {0}", string.Join(", ", jumpPointLine)));
			}
		}
	}
}

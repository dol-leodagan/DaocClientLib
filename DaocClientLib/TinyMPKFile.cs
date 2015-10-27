/*
 * DaocClientLib - Dark Age of Camelot Setup Ressources Wrapper
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
using System.Linq;

namespace DaocClientLib.MPK
{
	/// <summary>
	/// Represents a file stored in an MPK archive.
	/// </summary>
	public class TinyMPKFile
	{
		/// <summary>
		/// Byte From Extracted File
		/// </summary>
		private readonly byte[] _buf;
		
		/// <summary>
		/// Gets the unencrypted Data in the MPK
		/// </summary>
		public byte[] Data { get { return _buf; } }
		
		/// <summary>
		/// In-Archive File Name
		/// </summary>
		public string Name { get; protected set; }
		
		/// <summary>
		/// Constructs a new MPK file entry
		/// </summary>
		/// <param name="data">The uncompressed data of this file entry</param>
		/// <param name="filename">The file name</param>
		public TinyMPKFile(string filename, byte[] data)
		{
			Name = filename;
			_buf = data;
		}
	}
}

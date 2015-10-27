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
using System.IO;

using DaocClientLib;

using NUnit.Framework;

namespace DaocClientLib.Test
{
	/// <summary>
	/// ClientDataWrapperTest NUNIT Test Class
	/// </summary>
	[TestFixture]
	public class ClientDataWrapperTest
	{
		/// <summary>
		/// An Inexisting Path to Test
		/// </summary>
		public const string WrongClientPath = @"ZZ:\SomeWrongDir";
		
		/// <summary>
		/// Test Loading a null directory for Argument Exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestWrongDirectoryLoading()
		{
			string str = null;
			var test = new ClientDataWrapper(str);
		}
		
		/// <summary>
		/// Test Loading an inexisting directory Path for File Exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void TestWrongPathDirectoryLoading()
		{
			var test = new ClientDataWrapper(WrongClientPath);
		}
		
		public ClientDataWrapperTest()
		{
		}
	}
}

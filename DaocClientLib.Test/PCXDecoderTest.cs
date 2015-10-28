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

using System.Resources;
using System.Linq;
using System.IO;

using NUnit.Framework;

namespace DaocClientLib.Test
{
	/// <summary>
	/// PCXDecoder NUnit Test Class
	/// </summary>
	public class PCXDecoderTest
	{
		/// <summary>
		/// Store some test images
		/// </summary>
		byte[] PCX24;
		byte[] PCX8;
		byte[] PCX24RLE;
		byte[] PCX8RLE;
		#region setup
		/// <summary>
		/// Get Tests Images from Resources
		/// </summary>
		[SetUp]
		public void Init()
		{
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			var rm = new ResourceManager("DaocClientLib.Test.Properties.testpcxressources", asm);
			PCX24 = (byte[])rm.GetObject("grid24");
			PCX8 = (byte[])rm.GetObject("grid8");
			PCX24RLE = (byte[])rm.GetObject("grid24rle");
			PCX8RLE = (byte[])rm.GetObject("grid8rle");
		}
		#endregion
		
		#region test decoder
		[Test]
		public void TestPCXImageLoading8()
		{
			var image = new PCXDecoder(PCX8).PcxImage;
			
			for (int x = 0 ; x < image.Width ; x++)
			{
				for (int y = 0 ; y < image.Height ; y++)
				{
					if ((x == 0 && (y % 2) == 0) || (y == 0 && (x % 2) == 0))
					{
						Assert.AreEqual(0, image.GetPixel(x, y).R);
						Assert.AreEqual(0, image.GetPixel(x, y).G);
						Assert.AreEqual(0, image.GetPixel(x, y).B);
					}
					else
					{
						Assert.AreEqual(255, image.GetPixel(x, y).R);
						Assert.AreEqual(255, image.GetPixel(x, y).G);
						Assert.AreEqual(255, image.GetPixel(x, y).B);
					}
				}
			}
		}
		
		[Test]
		public void TestPCXImageLoading24()
		{
			var image = new PCXDecoder(PCX24).PcxImage;
			
			for (int x = 0 ; x < image.Width ; x++)
			{
				for (int y = 0 ; y < image.Height ; y++)
				{
					if ((x == 0 && (y % 2) == 0) || (y == 0 && (x % 2) == 0))
					{
						Assert.AreEqual(0, image.GetPixel(x, y).R);
						Assert.AreEqual(0, image.GetPixel(x, y).G);
						Assert.AreEqual(0, image.GetPixel(x, y).B);
					}
					else
					{
						Assert.AreEqual(255, image.GetPixel(x, y).R);
						Assert.AreEqual(255, image.GetPixel(x, y).G);
						Assert.AreEqual(255, image.GetPixel(x, y).B);
					}
				}
			}
		}
		
		[Test]
		public void TestPCXImageLoading8RLE()
		{
			var image = new PCXDecoder(PCX8RLE).PcxImage;
			
			for (int x = 0 ; x < image.Width ; x++)
			{
				for (int y = 0 ; y < image.Height ; y++)
				{
					if ((x == 0 && (y % 2) == 0) || (y == 0 && (x % 2) == 0))
					{
						Assert.AreEqual(0, image.GetPixel(x, y).R);
						Assert.AreEqual(0, image.GetPixel(x, y).G);
						Assert.AreEqual(0, image.GetPixel(x, y).B);
					}
					else
					{
						Assert.AreEqual(255, image.GetPixel(x, y).R);
						Assert.AreEqual(255, image.GetPixel(x, y).G);
						Assert.AreEqual(255, image.GetPixel(x, y).B);
					}
				}
			}
		}
		
		[Test]
		public void TestPCXImageLoading24RLE()
		{
			var image = new PCXDecoder(PCX24RLE).PcxImage;
			
			for (int x = 0 ; x < image.Width ; x++)
			{
				for (int y = 0 ; y < image.Height ; y++)
				{
					if ((x == 0 && (y % 2) == 0) || (y == 0 && (x % 2) == 0))
					{
						Assert.AreEqual(0, image.GetPixel(x, y).R);
						Assert.AreEqual(0, image.GetPixel(x, y).G);
						Assert.AreEqual(0, image.GetPixel(x, y).B);
					}
					else
					{
						Assert.AreEqual(255, image.GetPixel(x, y).R);
						Assert.AreEqual(255, image.GetPixel(x, y).G);
						Assert.AreEqual(255, image.GetPixel(x, y).B);
					}
				}
			}
		}
		#endregion
		
		public PCXDecoderTest()
		{
		}
	}
}

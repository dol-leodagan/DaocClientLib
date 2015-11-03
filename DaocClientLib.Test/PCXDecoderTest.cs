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
		
		#region test constructor
		/// <summary>
		/// Test Constructor with null Byte array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestPCXImageConstructorWithNullByteArray()
		{
			byte[] test = null;
			new PCXDecoder(test);
		}
		
		/// <summary>
		/// Test Constructor with null string
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestPCXImageConstructorWithNullString()
		{
			string test = null;
			new PCXDecoder(test);
		}
		
		/// <summary>
		/// Test Constructor with empty Byte array
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestPCXImageConstructorWithShortByteArray()
		{
			var test = new byte[0];
			new PCXDecoder(test);
		}
		
		/// <summary>
		/// Test Constructor with missing file
		/// </summary>
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void TestPCXImageConstructorWithWrongPath()
		{
			var test = "nonexistingimg.pcx";
			new PCXDecoder(test);
		}
		
		/// <summary>
		/// Test Constructor with no valid magic byte
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestPCXImageConstructorWithWrongByteArray()
		{
			var test = new byte[]{ 0 };
			new PCXDecoder(test);
		}
		
		/// <summary>
		/// Test Constructor with no valid Header
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestPCXImageConstructorWithWrongHeaderArray()
		{
			var test = new byte[]{ 0x0A };
			new PCXDecoder(test);
		}
		#endregion
		
		#region test decoder
		/// <summary>
		/// Test PCX Grid 8bpp no RLE
		/// </summary>
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
		
		/// <summary>
		/// Test PCX Grid 24bpp no RLE
		/// </summary>
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
		
		/// <summary>
		/// Test PCX Grid 8bpp with RLE
		/// </summary>
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
		
		/// <summary>
		/// Test PCX Grid 24bpp with RLE
		/// </summary>
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

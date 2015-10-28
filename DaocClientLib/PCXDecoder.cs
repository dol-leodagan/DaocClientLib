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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace DaocClientLib
{
	/// <summary>
	/// PCX File Decoder
	/// </summary>
	public sealed class PCXDecoder : IDisposable
	{
		#region PCXHEADER
		/// <summary>
		/// PCX Header Class
		/// </summary>
		private sealed class PCXHeader
		{
			/// <summary>
			/// Header Size
			/// </summary>
			public const int HeaderSize = 128;
			/// <summary>
			/// Header Data Byte Array
			/// </summary>
			public byte[] m_Data = new byte[HeaderSize];
			
			/// <summary>
			///  Manufacturer Byte (0A;)
			/// </summary>
			public byte Manufacturer { get { return m_Data[0]; } }
			/// <summary>
			/// Version : 0?PC Paintbrush 2.5  ??2?PC Paintbrush 2.8 ??5?PC Paintbrush 3.0
			/// </summary>
			public byte Version { get { return m_Data[1]; } }
			/// <summary>
			/// Encoding : 1RLE
			/// </summary>
			public byte Encoding { get { return m_Data[2]; } }
			/// <summary>
			/// Bits Depth
			/// </summary>
			public byte Bits_Per_Pixel { get { return m_Data[3]; } }
			
			public ushort Xmin { get { return BitConverter.ToUInt16(m_Data, 4); } }
			public ushort Ymin { get { return BitConverter.ToUInt16(m_Data, 6); } }
			public ushort Xmax { get { return BitConverter.ToUInt16(m_Data, 8); } }
			public ushort Ymax { get { return BitConverter.ToUInt16(m_Data, 10); } }
			/// <summary>
			/// Horizontal Res
			/// </summary>
			public ushort Hres1 { get { return BitConverter.ToUInt16(m_Data, 12); } }
			/// <summary>
			/// Vertical Res
			/// </summary>
			public ushort Vres1 { get { return BitConverter.ToUInt16(m_Data, 14); } }
			/// <summary>
			/// Palerre
			/// </summary>
			public byte[] Palette
			{
				get
				{
					byte[] _Palette = new byte[48];
					Array.Copy(m_Data,16,_Palette,0,48);
					return _Palette;
				}				
			}
			/// <summary>
			/// Reserved Byte
			/// </summary>
			public byte Reserved { get { return m_Data[64]; } }
			/// <summary>
			/// Colour Planes
			/// </summary>
			public byte Colour_Planes { get { return m_Data[65]; } }
			/// <summary>
			/// Bytes per Line
			/// </summary>
			public ushort Bytes_Per_Line { get { return BitConverter.ToUInt16(m_Data, 66); } }
			/// <summary>
			/// Palette Type
			/// </summary>
			public ushort Palette_Type { get { return BitConverter.ToUInt16(m_Data, 68); } }
			/// <summary>
			/// Filler Bytes
			/// </summary>
			public byte[] Filler
			{
				get
				{
					byte[] m_Bytes = new byte[58];
					Array.Copy(m_Data, 70, m_Bytes, 0, 58);
					return m_Bytes;
				}
			}
			/// <summary>
			/// Width
			/// </summary>
			public int Width { get { return Xmax - Xmin + 1; } }
			/// <summary>
			/// Height
			/// </summary>
			public int Height { get { return Ymax - Ymin + 1; } }

			/// <summary>
			/// Build PCXHeader from Byte Array
			/// </summary>
			public PCXHeader(byte[] p_Data)
			{
				if (p_Data.Length < HeaderSize)
					throw new ArgumentException("Given Byte Array is shorter than Header Size !", "p_Data");
				Array.Copy(p_Data, m_Data, HeaderSize);
			}			
		}
		#endregion
		
		#region members
		/// <summary>
		/// PCXHeader
		/// </summary>
		private PCXHeader m_Head;
		
		/// <summary>
		/// Resulting Bitmap Image
		/// </summary>
		public Bitmap PcxImage { get; private set; }
		#endregion

		#region constructors
		/// <summary>
		/// Decode a PCX Image from file path
		/// </summary>
		public PCXDecoder(string p_FileFullName)
		{
			if (p_FileFullName == null)
				throw new ArgumentNullException("p_FileFullName");
			if (!File.Exists(p_FileFullName))
				throw new FileNotFoundException("Given File does not exists !", p_FileFullName);
			
			Load(File.ReadAllBytes(p_FileFullName));
		}
		
		/// <summary>
		/// Decode PCX Image from byte array
		/// </summary>
		public PCXDecoder(byte[] p_Data)
		{
			if (p_Data == null)
				throw new ArgumentNullException("p_Data");
			if (p_Data.Length < PCXHeader.HeaderSize)
				throw new ArgumentException("Given Byte Array Argument is shorter than PCX Header Size !", "p_Data");
			
			Load(p_Data);
		}
		#endregion
		
		#region constants
		/// <summary>
		/// PCX File Magic Byte
		/// </summary>
		private const byte PCXMagicByte = 0x0A;
		
		/// <summary>
		/// PCX Color Depth Byte
		/// </summary>
		public enum PCXColorDepth {
			bpp24 = 3,
			bpp8 = 1,
		}
		#endregion
		
		#region Load File
		/// <summary>
		/// Load PCX Image from Byte Array
		/// </summary>
		/// <param name="p_Bytes">PCX File Byte Array</param>
		private void Load(byte[] p_Bytes)
		{
			// Copy Reference
			byte[] _Bytes = p_Bytes;
			// Check Magic Byte
			if (_Bytes[0] != PCXMagicByte)
				throw new ArgumentException(string.Format("Magic Byte is {0} instead of {1}", _Bytes[0], PCXMagicByte), "p_Bytes");
			
			// Get PCX header
			m_Head = new PCXHeader(_Bytes);
			var _Position = PCXHeader.HeaderSize;
			
			// Detect Depth
			PixelFormat _PixFormate = PixelFormat.Format24bppRgb;
			if (m_Head.Colour_Planes == (byte)PCXColorDepth.bpp8)
				_PixFormate = PixelFormat.Format8bppIndexed;
			
			// Create Image from Header Data
			PcxImage = new Bitmap(m_Head.Width, m_Head.Height, _PixFormate);
			BitmapData _Data = PcxImage.LockBits(new Rectangle(0, 0, PcxImage.Width, PcxImage.Height), ImageLockMode.ReadWrite, _PixFormate);
			
			byte[] _BmpData = new byte[_Data.Stride * _Data.Height];
			
			for (int i = 0; i < m_Head.Height; i++)
			{
				byte[] _RowColorValue = new byte[0];
				
				switch (m_Head.Colour_Planes)
				{
					case (byte)PCXColorDepth.bpp8: //256
						_RowColorValue = LoadPCXLine8(_Bytes, ref _Position);
						break;
					default: //24
						_RowColorValue = LoadPCXLine24(_Bytes, ref _Position);
						break;
				}
				
				int _Count = _RowColorValue.Length;
				Array.Copy(_RowColorValue, 0, _BmpData, i * _Data.Stride, _Count);
			}
			
			Marshal.Copy(_BmpData, 0, _Data.Scan0, _BmpData.Length);
			PcxImage.UnlockBits(_Data);
			
			switch (m_Head.Colour_Planes)
			{
				case (byte)PCXColorDepth.bpp8:
					ColorPalette _Palette = PcxImage.Palette;
					_Position = p_Bytes.Length - 256 * 3;
					for (int i = 0; i != 256; i++)
					{
						_Palette.Entries[i] = Color.FromArgb(p_Bytes[_Position], p_Bytes[_Position + 1], p_Bytes[_Position + 2]);
						_Position += 3;
					}
					PcxImage.Palette = _Palette;
					break;
			}
		}
		#endregion
						
		#region Load Lines
		/// <summary>
		/// PCX 24
		/// </summary>
		/// <param name="p_Data"></param>
		/// <param name="_Position"></param>
		/// <returns>BMP</returns>
		private byte[] LoadPCXLine24(byte[] p_Data, ref int _Position)
		{
			int _LineWidth = m_Head.Bytes_Per_Line;
			byte[] _ReturnBytes = new byte[_LineWidth * 3];
			int _EndBytesLength = p_Data.Length - 1;
			int _WriteIndex = 2;
			int _ReadIndex = 0;
			while (true)
			{
				if (_Position > _EndBytesLength) break; //
				byte _Data = p_Data[_Position];
				
				if (m_Head.Encoding > 0 && _Data > 0xC0)
				{
					int _Count = _Data - 0xC0;
					_Position++;
					for (int i = 0; i < _Count; i++)
					{
						if (i + _ReadIndex >= _LineWidth)          //2009-6-12 RLE
						{
							_WriteIndex--;
							_ReadIndex = 0;
							_Count = _Count - i;
							i = 0;
						}
						int _RVA = ((i + _ReadIndex) * 3) + _WriteIndex;
						_ReturnBytes[_RVA] = p_Data[_Position];
					}
					_ReadIndex += _Count;
					_Position++;
				}
				else
				{
					int _RVA = (_ReadIndex * 3) + _WriteIndex;
					_ReturnBytes[_RVA] = _Data;
					_Position++;
					_ReadIndex++;
				}
				
				if (_ReadIndex >= _LineWidth)
				{
					_WriteIndex--;
					_ReadIndex = 0;
				}
				
				if (_WriteIndex == -1) break;
			}
			return _ReturnBytes;
		}
		
		/// <summary>
		/// PCX 8
		/// </summary>
		/// <param name="p_Data"></param>
		/// <param name="_Position"></param>
		/// <returns>BMP</returns>
		private byte[] LoadPCXLine8(byte[] p_Data, ref int _Position)
		{
			int _LineWidth = m_Head.Bytes_Per_Line;
			byte[] _ReturnBytes = new byte[_LineWidth];
			int _EndBytesLength = p_Data.Length - 1 - (256 * 3);         //??
			int _ReadIndex = 0;
			while (true)
			{
				if (_Position > _EndBytesLength) break; //
				
				byte _Data = p_Data[_Position];
				if (m_Head.Encoding > 0 && _Data > 0xC0)
				{
					int _Count = _Data - 0xC0;
					_Position++;
					for (int i = 0; i < _Count; i++)
					{
						_ReturnBytes[i + _ReadIndex] = p_Data[_Position];
					}
					_ReadIndex += _Count;
					_Position++;
				}
				else
				{
					_ReturnBytes[_ReadIndex] = _Data;
					_Position++;
					_ReadIndex++;
				}
				if (_ReadIndex >= _LineWidth) break;
			}
			return _ReturnBytes;
		}
		#endregion
		
		#region IDisposable
		/// <summary>
		/// Dispose Image when IDisposable is triggered
		/// </summary>
		public void Dispose()
		{
			if (PcxImage != null)
				PcxImage.Dispose();
		}
		#endregion
	}
}
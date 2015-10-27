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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace DaocClientLib.MPK
{
	/// <summary>
	/// Handles the reading and writing to MPK files.
	/// </summary>
	public class TinyMPK : IEnumerable<KeyValuePair<string, TinyMPKFile>>
	{
		/// <summary>
		/// The magic at the top of the file
		/// </summary>
		private const uint Magic = 0x4b41504d; //MPAK

		/// <summary>
		/// CRC32 of the deflated directory
		/// </summary>
		private readonly Crc32 _crc = new Crc32();

		/// <summary>
		/// Holds all of the files in the MPK
		/// </summary>
		private readonly Dictionary<string, TinyMPKFile> _files = new Dictionary<string, TinyMPKFile>();

		/// <summary>
		/// Name of the archive
		/// </summary>
		private string _name = "";

		/// <summary>
		/// The name of this file
		/// </summary>
		public string Name { get { return _name; } }

		/// <summary>
		/// The filecount in this MPK
		/// </summary>
		public int Count { get { return _files.Count; } }
		
		/// <summary>
		/// Creates a new MPK Wrapper
		/// </summary>
		/// <param name="fname">The filename</param>
		public TinyMPK(string fname)
			: this(new FileInfo(fname))
		{
		}
		
		/// <summary>
		/// Creates a new MPK Wrapper
		/// </summary>
		/// <param name="file">The filename</param>
		public TinyMPK(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			
			if (!file.Exists)
				throw new FileNotFoundException("Could not find MPK File.", file.FullName);
			
			Read(file);
		}


		/// <summary>
		/// Gets a specific MPK file from this MPK
		/// </summary>
		public TinyMPKFile this[string fname]
		{
			get
			{
				if (_files.ContainsKey(fname.ToLower()))
				{
					return _files[fname.ToLower()];
				}

				return null;
			}
		}

		/// <summary>
		/// Gets a list of all the files inside this MPK
		/// </summary>
		/// <returns>An IDictionaryEnumerator containing entries as filename, MPKFile pairs</returns>
		public IEnumerator<KeyValuePair<string, TinyMPKFile>> GetEnumerator()
		{
			return _files.GetEnumerator();
		}
		
		/// <summary>
		/// Gets a list of all the files inside this MPK
		/// </summary>
		/// <returns>An IDictionaryEnumerator containing entries as filename, MPKFile pairs</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Reads a MPK file
		/// </summary>
		/// <param name="file">The MPK file to read</param>
		private void Read(FileInfo file)
		{
			using (var rdr = new BinaryReader(file.OpenRead(), Encoding.UTF8))
			{
				if (rdr.ReadUInt32() != Magic)
				{
					throw new ArgumentException("Invalid MPK file", "file");
				}

				rdr.ReadByte(); //always 2 unknown

				ReadArchive(rdr);
			}
		}

		/// <summary>
		/// Reads a MPK from a binary reader
		/// </summary>
		/// <param name="rdr">The binary reader pointing to the MPK</param>
		private void ReadArchive(BinaryReader rdr)
		{
			_files.Clear();

			_crc.Value = 0;

			// Read Header
			byte[] mpkHeader = new byte[16];
			for (int i = 0; i < mpkHeader.Length; i++) {
				mpkHeader[i] = (byte)((int)rdr.ReadByte() ^ i);
			}

			// Header Check
			uint crc;
			uint filesHeaderLength;
			uint nameLength;
			uint FileCount;
			
			unchecked
			{
				crc = (uint)((int)mpkHeader[0] + ((int)mpkHeader[1] << 8) + ((int)mpkHeader[2] << 16) + ((int)mpkHeader[3] << 24));
				filesHeaderLength = (uint)((int)mpkHeader[4] + ((int)mpkHeader[5] << 8) + ((int)mpkHeader[6] << 16) + ((int)mpkHeader[7] << 24));
				nameLength = (uint)((int)mpkHeader[8] + ((int)mpkHeader[9] << 8) + ((int)mpkHeader[10] << 16) + ((int)mpkHeader[11] << 24));
				FileCount = (uint)((int)mpkHeader[12] + ((int)mpkHeader[13] << 8) + ((int)mpkHeader[14] << 16) + ((int)mpkHeader[15] << 24));
			}
			
			// Get real name of Archive
			byte[] compressedName = rdr.ReadBytes((int)nameLength);
			byte[] name = Decompress(compressedName);
			_name = Encoding.UTF8.GetString(name, 0, name.Length);

			// Get Files header and CRC
			byte[] filesHeaderArray = rdr.ReadBytes((int)filesHeaderLength);
			byte[] sourceArray = rdr.ReadBytes((int)(rdr.BaseStream.Length - rdr.BaseStream.Position));
			_crc.Update(filesHeaderArray);
			
			// Check CRC
			if (_crc.Value != crc)
				throw new FileLoadException("MPAK CRC Mismatch !");
			
			// Unpack Files
			byte[] filesHeader = Decompress(filesHeaderArray);
			int fileIndex = 0;
			while (fileIndex < FileCount)
			{
				using (var memoryHeaderStream = new MemoryStream(filesHeader, fileIndex * 284, 284))
				{
					using (var binaryHeaderReader = new BinaryReader(memoryHeaderStream))
					{
						string fileName = GetNullTerminatedString(binaryHeaderReader.ReadBytes(256));
						uint timestamp = binaryHeaderReader.ReadUInt32();
						binaryHeaderReader.ReadUInt32();
						binaryHeaderReader.ReadUInt32();
						uint fileLength = binaryHeaderReader.ReadUInt32();
						uint fileOffset = binaryHeaderReader.ReadUInt32();
						uint compressedFileLength = binaryHeaderReader.ReadUInt32();
						uint num6 = binaryHeaderReader.ReadUInt32();
						byte[] compressedFileContent = new byte[compressedFileLength];
						Array.Copy(sourceArray, (long)fileOffset, compressedFileContent, 0L, (long)compressedFileLength);
						_crc.Reset();
						_crc.Update(compressedFileContent);
						if (_crc.Value != num6) {
							throw new FileLoadException("MPAK File CRC Mismatch !");
						}
						byte[] fileContent = Decompress(compressedFileContent);
						if (fileContent.Length !=fileLength) {
							throw new FileLoadException("MPAK File DataLength Mismatch !");
						}
						
						this._files.Add(fileName.ToLower(), new TinyMPKFile(fileName.ToLower(), fileContent));
					}
				}
				fileIndex++;
			}
			_crc.Reset();
		}
		
		/// <summary>
		/// Retrieve C-Style NullTerminated Strings
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private static string GetNullTerminatedString(byte[] src)
		{
			string @string;
			for (int i = 0; i < src.Length; i++) {
				if (src[i] == 0) {
					@string = Encoding.UTF8.GetString(src, 0, i);
					return @string;
				}
			}
			@string = Encoding.UTF8.GetString(src);
			return @string;
		}
		
		/// <summary>
		/// Decompress an Array Byte using ZipLib Inflate
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private static byte[] Decompress(byte[] src)
		{
			Inflater inflater = new Inflater();
			List<byte> list = new List<byte>();
			using (var memoryStream = new MemoryStream(src))
			{
				while (!inflater.IsFinished)
				{
					while (inflater.IsNeedingInput)
					{
						int remaining = (int)(memoryStream.Length - memoryStream.Position);
						if (remaining <= 0)
						{
							Console.WriteLine("Error while decompressing - EOF!");
							return list.ToArray();
						}
						int bufferSize = (remaining > 2048) ? 2048 : remaining;
						byte[] array = new byte[bufferSize];
						memoryStream.Read(array, 0, bufferSize);
						inflater.SetInput(array);
					}
					byte[] array2 = new byte[2048];
					int num3 = inflater.Inflate(array2);
					if (num3 != array2.Length)
					{
						Array.Resize<byte>(ref array2, num3);
					}
					list.AddRange(array2);
				}
			}
			return list.ToArray();
		}
	}
}

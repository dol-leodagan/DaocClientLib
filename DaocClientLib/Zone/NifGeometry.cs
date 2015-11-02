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

namespace DaocClientLib
{
	/// <summary>
	/// NifGeometry Describe Transformation for a given Nif Mesh identified by a file name
	/// </summary>
	public sealed class NifGeometry
	{
		/// <summary>
		/// Nif Arbitrary ID
		/// </summary>
		public int ID { get; private set; }
		
		/// <summary>
		/// File Name
		/// </summary>
		public string FileName { get; private set; }
		
		/// <summary>
		/// Textual Name
		/// </summary>
		public string TextualName { get; private set; }
		
		/// <summary>
		/// Translation X
		/// </summary>
		public float X { get; private set; }
		/// <summary>
		/// Translation Y
		/// </summary>
		public float Y { get; private set; }
		/// <summary>
		/// Translation Z
		/// </summary>
		public float Z { get; private set; }
		/// <summary>
		/// Scale
		/// </summary>
		public float Scale { get; private set; }
		/// <summary>
		/// Rotation Angle
		/// </summary>
		public float Angle { get; private set; }
		/// <summary>
		/// Rotation X
		/// </summary>
		public float RotationX { get; private set; }
		/// <summary>
		/// Rotation Y
		/// </summary>
		public float RotationY { get; private set; }
		/// <summary>
		/// Rotation Z
		/// </summary>
		public float RotationZ { get; private set; }
		/// <summary>
		/// Flip Meshes ?
		/// </summary>
		public bool Flip { get; private set; }
		/// <summary>
		/// Is Mapped to Ground ?
		/// </summary>
		public bool OnGround { get; private set; }
		/// <summary>
		/// Relatively Placed to Other Nif
		/// </summary>
		public NifGeometry RelativeTo { get; private set; }
		
		/// <summary>
		/// Default Constructor
		/// </summary>
		public NifGeometry(int id, string file, string textual, float x, float y, float z, float scale, float angle, float rotationX, float rotationY, float rotationZ, bool flip, bool ground, NifGeometry relative)
		{
			ID = id;
			FileName = file;
			TextualName = textual;
			X = x;
			Y = y;
			Z = z;
			Scale = scale;
			Angle = angle;
			RotationX = rotationX;
			RotationY = rotationY;
			RotationZ = rotationZ;
			Flip = flip;
			OnGround = ground;
			RelativeTo = relative;
		}
	}
}

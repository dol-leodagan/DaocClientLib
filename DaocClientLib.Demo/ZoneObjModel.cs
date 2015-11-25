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

// Don't Compile Demo if in an Unsupported Configuration
#if OpenTK || Debug

namespace DaocClientLib.Demo
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using OpenTK;
	using OpenTK.Graphics.OpenGL;

	using SharpNav.Geometry;
	
	using DaocClientLib.Drawing;
	using Niflib.Extensions;

	public enum ModelType
	{
		Pickee,
		Collidee,
		Visible,
	}
	
	/// <summary>
	/// Helper Class for Drawing Zone Renderer
	/// </summary>
	public class ZoneObjModel
	{
		/// <summary>
		/// Total Triange count for this Zone Model
		/// </summary>
		public int TriangleCount { get; protected set; }
		/// <summary>
		/// Pre Calculated Bounding Box for this Zone Model
		/// </summary>
		public BBox3 BoundingBox { get; protected set; }
		/// <summary>
		/// ZoneRenderer Object for Extracting Primitives from Zone
		/// </summary>
		public ZoneRenderer Renderer { get; protected set; }
		
		private ModelType m_type;
		/// <summary>
		/// Zone Model Layer Type
		/// </summary>
		public ModelType Type { get { return m_type; } set { m_type = value; Update(); } }
		
		/// <summary>
		/// Array of Vertex Buffer index.
		/// </summary>
		protected uint[] VBOids;
		/// <summary>
		/// Array of Normal Buffer index.
		/// </summary>
		protected uint[] NBOids;
		/// <summary>
		/// Array of Index buffer index.
		/// </summary>
		protected uint[] IBOids;
		
		/// <summary>
		/// Dictionary Resolving Graphic buffer index to Nif Buffer Index (and Triangle count)
		/// </summary>
		protected Dictionary<int, KeyValuePair<int, int>> NifIdBufferIndex;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ZoneObjModel"/> class.
		/// </summary>
		/// <param name="renderer">Zone Primitive Renderer</param>
		public ZoneObjModel(ZoneRenderer renderer)
		{
			Renderer = renderer;
			Type = ModelType.Visible;
		}
		
		/// <summary>
		/// Update/Create Graphic Buffer index and store current Meshes Data 
		/// </summary>
		protected void Update()
		{
			TriangleCount = 0;
			var objectCount = Renderer.NifCache.Count;
			// Init VBOs
			if (VBOids == null)
			{
				VBOids = new uint[objectCount];
				GL.GenBuffers(objectCount, VBOids);
			}
			
			// Init NBOs
			if (NBOids == null)
			{
				NBOids = new uint[objectCount];
				GL.GenBuffers(objectCount, NBOids);
			}
			
			// Init IBOs
			if (IBOids == null)
			{
				IBOids = new uint[objectCount];
				GL.GenBuffers(objectCount, IBOids);
			}
			
			// init Nif index
			if (NifIdBufferIndex == null)
			{
				var index = 0;
				NifIdBufferIndex = Renderer.NifCache.ToDictionary(kv => kv.Key, kv => new KeyValuePair<int, int>(index++, 0));
			}
			
			switch(Type)
			{
				case ModelType.Pickee:
					key = "pickee";
					break;
				case ModelType.Collidee:
					key = "collidee";
					break;
				case ModelType.Visible:
				default:
					key = "visible";
					break;
			}
			
			foreach (var mesh in Renderer.NifCache)
			{						
				TriangleCollection tris;
				switch(Type)
				{
					case ModelType.Pickee:
						tris = mesh.Value.Pickee;
						break;
					case ModelType.Collidee:
						tris = mesh.Value.Collidee;
						break;
					case ModelType.Visible:
					default:
						tris = mesh.Value.Visible;
						break;
				}
								
				KeyValuePair<int, int> buffer;
				if (!NifIdBufferIndex.TryGetValue(mesh.Key, out buffer))
					continue;
				
				// Retrieve Normals for Lighting
				var norms = tris.ComputeNormalLighting();
				
				// Bind Vertices Buffer
				GL.BindBuffer(BufferTarget.ArrayBuffer, VBOids[buffer.Key]);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tris.Vertices.Length * 3 * 4), tris.Vertices, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				// Bind Normals Buffer
				GL.BindBuffer(BufferTarget.ArrayBuffer, NBOids[buffer.Key]);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(norms.Length * 3 * 4), norms, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				// Bind Indices Buffer
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOids[buffer.Key]);
				GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(tris.Indices.Length * 3 * 4), tris.Indices, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				// Set Indices Count
				NifIdBufferIndex[mesh.Key] = new KeyValuePair<int, int>(buffer.Key, tris.Indices.Length * 3);
				// Set Triangles Count
				TriangleCount += tris.Indices.Length * Renderer.InstancesMatrix.Count(i => i.Key == mesh.Key);
			}
			
			BoundingBox = Triangles().GetBoundingBox();
		}
		
		/// <summary>
		/// Draw the Zone Object Model from content in Graphic Buffers
		/// </summary>
		public void Draw()
		{
			if (VBOids == null || NBOids == null || IBOids == null)
				return;

			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);

			GL.Enable(EnableCap.Normalize);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0.5f, 1, 0.5f, 0));			

			foreach (var mesh in Renderer.InstancesMatrix)
			{
				var trans = mesh.Value;
				
				// check if the Mesh has been inverted to change Face Direction
				if (trans.Determinant < 0)
					GL.FrontFace(FrontFaceDirection.Cw);
				else
					GL.FrontFace(FrontFaceDirection.Ccw);
				
				KeyValuePair<int, int> buffer;
				if (!NifIdBufferIndex.TryGetValue(mesh.Key, out buffer))
					continue;
				
				var vbo = VBOids[buffer.Key];
				var nbo = NBOids[buffer.Key];
				var ibo = IBOids[buffer.Key];

				GL.PushMatrix();
				GL.MultMatrix(ref trans);
	
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
				GL.VertexPointer(3, VertexPointerType.Float, 3 * 4, 0);

				GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
				GL.NormalPointer(NormalPointerType.Float, 3 * 4, 0);
				
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
				GL.DrawElements(PrimitiveType.Triangles, buffer.Value, DrawElementsType.UnsignedInt, 0);
	
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
	
				GL.PopMatrix();
			}

			GL.Disable(EnableCap.Normalize);
			GL.Disable(EnableCap.Light0);
			GL.Disable(EnableCap.Lighting);

			GL.DisableClientState(ArrayCap.NormalArray);
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.FrontFace(FrontFaceDirection.Ccw);
		}
		
		/// <summary>
		/// Unload and Clean Graphic Buffers
		/// </summary>
		public void Unload()
		{
			foreach (var buff in VBOids)
				GL.DeleteBuffer(buff);
			foreach (var buff in NBOids)
				GL.DeleteBuffer(buff);
			foreach (var buff in IBOids)
				GL.DeleteBuffer(buff);
		}
		
		/// <summary>
		/// Enumerate all Triangles from current Zone Model Type
		/// (Can be used for NavMesh computing)
		/// </summary>
		/// <returns>Raw Triangle Enumerable of All instances in the Zone</returns>
		public IEnumerable<Triangle3> Triangles()
		{
			foreach (var mesh in Renderer.InstancesMatrix)
			{
				ClientMesh layers;
				if (!Renderer.NifCache.TryGetValue(mesh.Key, out layers))
					continue;

				TriangleCollection tris;
				switch(Type)
				{
					case ModelType.Pickee:
						tris = layers.Pickee;
						break;
					case ModelType.Collidee:
						tris = layers.Collidee;
						break;
					case ModelType.Visible:
					default:
						tris = layers.Visible;
						break;
				}
				var matrix = mesh.Value;
				
				foreach (var tri in tris.AsEnumerable<Triangle3>((a, b, c) =>
				                                                 {
				                                                 	Vector3 transA;
				                                                 	Vector3 transB;
				                                                 	Vector3 transC;
				                                                 	Vector3.Transform(ref a, ref matrix, out transA);
				                                                 	Vector3.Transform(ref b, ref matrix, out transB);
				                                                 	Vector3.Transform(ref c, ref matrix, out transC);
				                                                 	return new Triangle3(transA, transB, transC);
				                                                 }))
					yield return tri;
			}
			yield break;
		}
	}
}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Sandvox
{
    public class InstancedFaceBatcher
        {
            public class BatchBuffer : ICloneable
            {
                public bool NeedsUpdating = true;
                public int Count;
                int Position;
                int Size;
                public FaceInstance[] Faces;
                public VertexBufferBinding[] bindings = new VertexBufferBinding[2];
                public VertexBuffer geometryBuffer;
                public IndexBuffer indexBuffer;
                public VertexBuffer instanceBuffer;
                public bool Full = false;
                public bool Drawn = false;
                public bool Control = false;
                public bool Updating = false;
                public bool Drawing = false;
                private readonly object Lock = new object();


                public BatchBuffer(int size)
                {
                    Size = size;
                    // Initialize the instance buff.
                    Faces = new FaceInstance[size];


                    // Initialize control variables.
                    Position = 0;
                    Count = 0;
                    //Buffer = new VertexBuffer(Game.graphics.GraphicsDevice, FaceInstance.VertexDeclaration, Count, BufferUsage.WriteOnly);
                    NeedsUpdating = false;

                }
                public int AddFace(FaceInstance tmp)
                {
                    if (!Seek()) return -1;

                    Faces[Position] = tmp;

                    Count++;
                    NeedsUpdating = true;
                    Full = (Count == Size);
                    return Position;
                }
                public void RemoveFace(int index)
                {
                    Position = index;
                    if (Faces[index] == Face.Zero) return;
                    Faces[index] = Face.Zero;
                    Count--;
                    Full = false;
                    NeedsUpdating = true;
                }
                public void Update()
                {
                    if (Drawing ) return;
                    Updating = true;

                    NakedUpdate();
                    
                    Updating = false;
                }
                public void NakedUpdate()
                {
                    // Initialize the vertex buffer. Should only hold a face.
                    geometryBuffer = new VertexBuffer(BlockGame.graphics.GraphicsDevice, Vertex.VertexDeclaration, Face.Vertices.Length, BufferUsage.WriteOnly);

                    indexBuffer = new IndexBuffer(BlockGame.graphics.GraphicsDevice, typeof(short), Face.Indices.Length, BufferUsage.WriteOnly);

                    instanceBuffer = new VertexBuffer(BlockGame.graphics.GraphicsDevice, FaceInstance.VertexDeclaration, Size, BufferUsage.WriteOnly);

                    // Set the indices
                    indexBuffer.SetData<short>(Face.Indices);

                    // Set the buffer to contain the face.
                    geometryBuffer.SetData<Vertex>(Face.Vertices);

                    // Set buffer to the new faces.
                    instanceBuffer.SetData<FaceInstance>(Faces, 0, Size);

                    // Set the buffer.
                    BlockGame.graphics.GraphicsDevice.Indices = indexBuffer;

                    bindings = new VertexBufferBinding[2];
                    bindings[0] = new VertexBufferBinding(geometryBuffer);
                    bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
                    Console.WriteLine("Updated buffer!");
                    NeedsUpdating = false;
                    Drawn = false;
                }
                public void Draw()
                {
                    //if () return;
                    // No buffer? No draw!
                    if (Updating || Count == 0 || instanceBuffer == null) return; //|| Control

                    Drawing = true;

                    //if (NeedsUpdating) NakedUpdate();
                    if (Drawn == false) Console.WriteLine("Drawing for the first time! Yay!");

                    BlockGame.graphics.GraphicsDevice.SetVertexBuffers(bindings);

                    // Draw!
                    BlockGame.graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, Face.Indices.Length / 3, Size);


                    //Game.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Size / 3);

                    Drawn = true;
                    Drawing = false;
                }
                public bool Seek()
                {
                    // It's full! Can't find an open spot.
                    if (Full) return false;

                    // No vertices? No problem!
                    if (Count == 0) return true;

                    // Otherwise LOCATE OPENING!!
                    for (int i = 0; i < Size; i++)
                    {
                        if (Faces[Position] == Face.Zero) return true;
                        Position = (Position + 1) % Size;
                    }

                    Full = true;

                    // Bleh. Couldn't find one :/
                    return false;
                }
                public object Clone()
                {
                    return this.MemberwiseClone();
                }
            }
            const int DefaultSize = 20000;
            int Size = DefaultSize;
            List<BatchBuffer> buffers;
            BatchBuffer backup;
            bool DrawBackup = false;
            VertexBufferBinding[] bindings = new VertexBufferBinding[2];
            public VertexBuffer vertexBuffer;
            public IndexBuffer indexBuffer;

            public InstancedFaceBatcher()
            {
                buffers = new List<BatchBuffer>();
            }
            public int AddFace(Face.Facing facing, float x, float y, float z, Vector3 size, ref Color color)
            {
                if (color == Color.Black) Console.WriteLine("Black detected.");
                for (int i = 0; i < buffers.Count; i++)
                {
                    if (!buffers[i].Full)
                    {
                        //int k = buffers[i].AddFace(Face.getFace(facing, new Vector3(x, y, z), size, color.ToVector3()));
                        int k = buffers[i].AddFace(Face.getInstancedFace(facing, new Vector3(x, y, z), size, color.ToVector3()));

                        if (k > -1)
                            return (i * Size) + k;

                        return -1;
                    }
                }

                buffers.Add(new BatchBuffer(Size));

                int j = buffers[buffers.Count - 1].AddFace(Face.getInstancedFace(facing, new Vector3(x, y, z), size, color.ToVector3()));

                if (j > -1)
                    return ((buffers.Count - 1) * Size) + j;

                return -1;
            }
            public int AddFace(Face.Facing facing, ref Vector3 position, Vector3 size, ref Color color)
            {
                return this.AddFace(facing, position.X, position.Y, position.Z, size, ref color);
            }
            public void RemoveFace(int index)
            {
                if (index < 0) return;
                buffers[index / Size].RemoveFace(index % Size);

            }
            public void Update()
            {
                for (int i = 0; i < buffers.Count; i++)
                {
                    if (buffers[i].NeedsUpdating) //&& buffers[i].Drawn == false
                    {
                        //buffers[i].Control = true;
                        //buffers[i].Control = false;
                        backup = (BatchBuffer)buffers[i].Clone();
                        DrawBackup = true;

                        
                        buffers[i].Update();
                        //buffers[i].NeedsUpdating = false;
                        //buffers[i].Drawn = false;
                        
                        //buffers[i].Control = false;
                        DrawBackup = false;
                    }
                }
            }
            public void Draw()
            {
                // Set up the effect parameters.
                BlockGame.effect.Parameters["worldViewProjectionMatrix"].SetValue(BlockGame.WorldViewProjection);

                // Apply the effect.
                BlockGame.effect.CurrentTechnique.Passes[0].Apply();

                for (int i = 0; i < buffers.Count; i++)
                {
                    //if (buffers[i].bindings == null || buffers[i].Updating ) continue;

                    

                    buffers[i].Draw();
                }
                if (DrawBackup)
                {
                    backup.Draw();
                }
            }
            public int Count()
            {
                int j = 0;
                for (int i = 0; i < buffers.Count; i++)
                {
                    j += buffers[i].Count;
                }
                return j;
            }
        }
    
}

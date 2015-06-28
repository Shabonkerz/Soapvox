using System;
using System.Collections.Generic;
using System.Linq;
using Sandvox;
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
    public class BatchBuffer : ICloneable
    {
        public bool Updated;
        public int Count;
        int Position;
        public VertexBuffer Buffer;
        int Size;
        public VertexPositionNormalColor[] Faces;
        public bool Full = false;
        public bool Drawn = false;
        public bool Control = false;
        private readonly object Lock = new object();


        public BatchBuffer(int size)
        {
            Size = size;
            Faces = new VertexPositionNormalColor[size];
            Buffer = new VertexBuffer(Game.graphics.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, Size, BufferUsage.WriteOnly);
            Buffer.SetData<VertexPositionNormalColor>(Faces);
            Position = 0;
            Count = 0;
            Updated = true;

        }
        public int AddVertices(VertexPositionNormalColor[] tmp)
        {
            if (!Seek()) return -1;

            Faces[Position] = tmp[0];
            Faces[Position + 1] = tmp[1];
            Faces[Position + 2] = tmp[2];
            Faces[Position + 3] = tmp[3];
            Faces[Position + 4] = tmp[4];
            Faces[Position + 5] = tmp[5];

            Count += 6;
            Updated = true;
            Full = (Count == Size);
            return Position;
        }
        public void RemoveVertices(int index)
        {
            if (Faces[index].Normal == Vector3.Zero) return;
            Faces[index] = VertexPositionNormalColor.Zero;
            Faces[index + 1] = VertexPositionNormalColor.Zero;
            Faces[index + 2] = VertexPositionNormalColor.Zero;
            Faces[index + 3] = VertexPositionNormalColor.Zero;
            Faces[index + 4] = VertexPositionNormalColor.Zero;
            Faces[index + 5] = VertexPositionNormalColor.Zero;
            Count -= 6;
            Full = false;
            Updated = true;
        }
        public void Update()
        {

            Buffer = new VertexBuffer(Game.graphics.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, Size, BufferUsage.WriteOnly);

            Buffer.SetData<VertexPositionNormalColor>(Faces);

        }
        public void Draw()
        {

            // No buffer? No draw!
            if (Buffer == null || Control) return;

            // Set up the effect parameters.
            Game.effect.Parameters["worldViewProjectionMatrix"].SetValue(Game.WorldViewProjection);

            // Apply the effect.
            Game.effect.CurrentTechnique.Passes[0].Apply();

            // Set the buffer.
            Game.graphics.GraphicsDevice.SetVertexBuffer(Buffer);

            // Draw!
            Game.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Size / 3);

            this.Drawn = true;

        }
        public bool Seek()
        {
            // It's full! Can't find an open spot.
            if (Full) return false;

            // No vertices? No problem!
            if (Count == 0) return true;

            // Otherwise LOCATE OPENING!!
            for (int i = 0; i < Size; i += 6)
            {
                if (Faces[Position].Normal == Vector3.Zero) return true;
                Position = (Position + 6) % Size;
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
    public class FaceBatch
    {
        const int DefaultSize = 60000;
        int Size = DefaultSize;
        List<BatchBuffer> buffers;
        BatchBuffer backup;
        bool DrawBackup = false;

        public FaceBatch()
        {
            buffers = new List<BatchBuffer>();
        }
        public int AddFace(Face.Facing facing, float x, float y, float z, Vector3 size, ref Color color)
        {
            for (int i = 0; i < buffers.Count; i++)
            {
                if (!buffers[i].Full)
                {
                    int k = buffers[i].AddVertices(Face.getFace(facing, new Vector3(x, y, z), size, color.ToVector3()));

                    if (k > -1)
                        return (i * Size) + k;

                    return -1;
                }
            }

            buffers.Add(new BatchBuffer(Size));

            int j = buffers[buffers.Count - 1].AddVertices(Face.getFace(facing, new Vector3(x, y, z), size, color.ToVector3()));

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
            buffers[index / Size].RemoveVertices(index % Size);
            
        }
        public void Update()
        {
            for (int i = 0; i < buffers.Count; i++)
            {
                if (!buffers[i].Updated || !buffers[i].Drawn) continue;
                buffers[i].Control = false;
                backup = (BatchBuffer)buffers[i].Clone();
                DrawBackup = true;
                buffers[i].Control = true;
                buffers[i].Update();
                buffers[i].Updated = false;
                buffers[i].Drawn = false;
                buffers[i].Control = false;
                DrawBackup = false;
            }
        }
        public void Draw()
        {
            for (int i = 0; i < buffers.Count; i++)
            {
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

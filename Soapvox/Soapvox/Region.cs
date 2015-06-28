using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Sandvox
{
	class Region
	{
        Vector3 size;
        Color color;
        public Vector3 position;
        public Matrix Translation = new Matrix();
        private List<VoxelFace> Faces;
        private VertexPositionNormalColor[] Vertices;


        DynamicVertexBuffer vertexbuffer;

        IndexBuffer indexbuffer;


        public static Texture2D Texture = Game.voxelColors;

        public Region( Vector3 size, Color color, Vector3 position )
        {
            this.size = size;
            this.color = color;
            this.position = position;
            Translation = Matrix.CreateTranslation(position); 

            Vertices = new VertexPositionNormalColor[]{

                // Front Surface
                new VertexPositionNormalColor(new Vector3(0,0,size.Z), Vector3.Backward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,size.Y,size.Z), Vector3.Backward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,0,size.Z), Vector3.Backward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,size.Z), Vector3.Backward, new Vector3(color.R,color.G,color.B)),

                // Back Surface
                new VertexPositionNormalColor(new Vector3(size.X,0,0), Vector3.Forward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,0), Vector3.Forward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,0,0), Vector3.Forward, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,size.Y,0), Vector3.Forward, new Vector3(color.R,color.G,color.B)),

                // Left Surface
                new VertexPositionNormalColor(new Vector3(0,0,0), Vector3.Left, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,size.Y,0), Vector3.Left, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,0,size.Z), Vector3.Left, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,size.Y,size.Z), Vector3.Left, new Vector3(color.R,color.G,color.B)),
            
                // Right Surface
                new VertexPositionNormalColor(new Vector3(size.X,0,size.Z), Vector3.Right, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,size.Z), Vector3.Right, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,0,0), Vector3.Right, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,0), Vector3.Right, new Vector3(color.R,color.G,color.B)),

                // Top Surface
                new VertexPositionNormalColor(new Vector3(0,size.Y,size.Z), Vector3.Up, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,size.Y,0), Vector3.Up, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,size.Z), Vector3.Up, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,size.Y,0), Vector3.Up, new Vector3(color.R,color.G,color.B)),

                // Bottom Surface
                new VertexPositionNormalColor(new Vector3(0,0,0), Vector3.Down, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(0,0,size.Z), Vector3.Down, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,0,0), Vector3.Down, new Vector3(color.R,color.G,color.B)),
                new VertexPositionNormalColor(new Vector3(size.X,0,size.Z), Vector3.Down, new Vector3(color.R,color.G,color.B))

            };

            vertexbuffer = new DynamicVertexBuffer(Game.graphics.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, 24, BufferUsage.WriteOnly);
            indexbuffer = new IndexBuffer(Game.graphics.GraphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            vertexbuffer.SetData<VertexPositionNormalColor>(this.Vertices);
            indexbuffer.SetData<short>(Voxel.indices);
        }
        public void setColor(Color color)
        {
            this.color = color;
        }
        public void Update()
        {
            Translation = Matrix.CreateTranslation(position);
        }
        public void Draw()
        { 
            Game.graphics.GraphicsDevice.SetVertexBuffer(vertexbuffer);
            Game.graphics.GraphicsDevice.Indices = indexbuffer;
            Game.effect.Parameters["worldViewProjectionMatrix"].SetValue(this.Translation * Game.WorldViewProjection);
            Game.effect.CurrentTechnique.Passes[0].Apply();
            Game.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
        }
	}
}

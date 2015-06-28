using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandvox
{
    class VoxelFace
    {
        public enum Face
        {
            Up, North, South, East, West, Down
        }
        
        VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
        Color Color;
        
        public VoxelFace(Color color, Vector3 Position, Face facing)
        {
            switch (facing)
            {
                case Face.Up:
                    vertices[0] = new VertexPositionNormalTexture( Position + new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0,0) );
                    vertices[1] = new VertexPositionNormalTexture( Position + new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0,0) );
                    vertices[2] = new VertexPositionNormalTexture( Position + new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0,0) );
                    vertices[3] = new VertexPositionNormalTexture( Position + new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0,0) );
                    break;
                case Face.Down:
                    vertices[0] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[1] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[2] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[3] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    break;
                case Face.North:
                    vertices[0] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[1] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[2] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[3] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    break;
                case Face.South:
                    vertices[0] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[1] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[2] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[3] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    break;
                case Face.East:
                    vertices[0] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[1] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[2] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[3] = new VertexPositionNormalTexture(Position + new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    break;
                case Face.West:
                    vertices[0] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[1] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[2] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 0));
                    vertices[3] = new VertexPositionNormalTexture(Position + new Vector3(-0.5f, 0.5f, -0.5f), Vector3.Up, new Vector2(0, 0));
                    break;
            }
        }
    }
}

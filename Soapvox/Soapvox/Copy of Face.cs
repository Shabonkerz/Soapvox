using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandvox
{
    public struct BasicFace
    {
        public Face.Facing facing;
        public VertexPositionNormalColor[] vertices;
        public int arrayPosition;

        public BasicFace( Face.Facing facing, Vector3 position, int size, Color color )
        {
            this.arrayPosition = 0;
            this.facing = facing;
            vertices = Face.getFace(  facing, position, size, color.ToVector3() ) ;
        }
        public static bool operator ==(BasicFace a, BasicFace b)
        {
            if (a.facing != b.facing) return false;
            return true;
        }
        public static bool operator !=(BasicFace a, BasicFace b)
        {
            if (a.facing == b.facing) return true;
            return false;
        }

    }
    public class Face
    {
        public enum Facing
        {
            Backward = 0,
            Forward = 1,
            Left = 2,
            Right = 3,
            Up = 4,
            Down = 5
        }
        public static Facing getFacing( VertexPositionNormalColor[] face )
        {
            if( face[0].Normal == Vector3.Backward ) return Facing.Forward;
            if( face[0].Normal == Vector3.Forward ) return Facing.Backward;
            if( face[0].Normal == Vector3.Left ) return Facing.Right;
            if( face[0].Normal == Vector3.Right ) return Facing.Left;
            if( face[0].Normal == Vector3.Down ) return Facing.Up;
            return Facing.Down;
        }
        public static Facing getFacing( List<VertexPositionNormalColor> faces )
        {
            return getFacing( faces.ToArray() );
        }
        public static VertexPositionNormalColor[] getFace( Facing facing, int size, ref Voxel voxel )
        {
            return getFace( facing, voxel.position, size, voxel.color);
        }
        public static VertexPositionNormalColor[] getFace( Facing facing, Vector3 Position, int size, Vector3 Color)
        {
           

            switch (facing)
            {
                case Facing.Forward:
                    return new VertexPositionNormalColor[]
                    {
                        // Front Surface
                        new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Backward, Color),
                        new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Backward, Color), 
                        new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Backward, Color),
                        new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Backward, Color), 
                        new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Backward, Color),
                        new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Backward, Color),  
                    };
                case Facing.Backward:
                    return new VertexPositionNormalColor[]
                    {
                        // Back Surface
                        new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Forward, Color),
                        new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Forward, Color), 
                        new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Forward, Color),
                        new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Forward, Color), 
                        new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Forward, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Forward, Color), 
                    };
                case Facing.Right:
                    return new VertexPositionNormalColor[]
                    {
                        // Right Surface
                        new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Left, Color),
                        new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Left, Color),
                        new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Left, Color),
                        new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Left, Color),
                        new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Left, Color),
                        new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Left, Color),
                    };
                case Facing.Left:
                    return new VertexPositionNormalColor[]
                    {            
                        // Left Surface
                        new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Right, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Right, Color),
                        new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Right, Color),
                        new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Right, Color),
                        new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Right, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Right, Color),
                    };
                case Facing.Down:
                    return new VertexPositionNormalColor[]
                    {
                        // Bottom Surface
                        new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Down, Color),
                        new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Down, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Down, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Down, Color),
                        new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Down, Color),
                        new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Down, Color),
                        
                    };
                case Facing.Up:
                    return new VertexPositionNormalColor[]
                    {
                        // Top Surface
                        new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Up, Color),
                        new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Up, Color),
                        new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Up, Color),
                        new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Up, Color),
                        new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Up, Color),
                        new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Up, Color),
            
                    };
                default: return new VertexPositionNormalColor[] { };
            }
        }
    }
}

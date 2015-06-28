using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandvox
{
    public struct FaceInstance : IVertexType
    {
        public Matrix World;
        public Vector3 Color;
        public Vector3 Normal;

        public static bool operator==(FaceInstance a, FaceInstance b)
        {
            return (
                    a.World == b.World &&
                    a.Color == b.Color &&
                    a.Normal == b.Normal);
        }
        public static bool operator !=(FaceInstance a, FaceInstance b)
        {
            return (
                    a.World != b.World ||
                    a.Color != b.Color || 
                    a.Normal != b.Normal);
        }
        public FaceInstance( Matrix world,
            Vector3 color, Vector3 normal)
        {
            World = world;
            Color = color;
            Normal = normal;
        }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
                new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.Color, 0),
                new VertexElement(76, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            }
        );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };



    //public struct BasicFace
    //{
    //    public Face.Facing facing;
    //    public VertexPositionNormalColor[] vertices;
    //    public int arrayPosition;

    //    public BasicFace( Face.Facing facing, Vector3 position, Vector3 size, Color color )
    //    {
    //        this.arrayPosition = 0;
    //        this.facing = facing;
    //        vertices = Face.getFace(  facing, position, size, color.ToVector3() ) ;
    //    }
    //    public static bool operator ==(BasicFace a, BasicFace b)
    //    {
    //        if (a.facing != b.facing) return false;
    //        return true;
    //    }
    //    public static bool operator !=(BasicFace a, BasicFace b)
    //    {
    //        if (a.facing == b.facing) return true;
    //        return false;
    //    }

    //}
    public class Cube
    {
        public readonly static Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0.5f);
        public readonly static Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);

        public readonly static Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0.5f);
        public readonly static Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);

        public readonly static Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0.5f);
        public readonly static Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);

        public readonly static Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0.5f);
        public readonly static Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);

        public readonly static Vertex[] Vertices = new Vertex[]
        {
            new Vertex( topLeftFront ), //0
            new Vertex( topLeftBack ), //1
            new Vertex( topRightFront ), //2
            new Vertex( topRightBack ), //3

            new Vertex( bottomRightFront ), //4 
            new Vertex( bottomRightBack ), //5 
            new Vertex( bottomLeftFront ), // 6
            new Vertex( bottomLeftBack ) // 7
            

        };
        public readonly static short[] Indices = new short[] {  
                                
                                // Front
                                6, 0, 2, 
                                2, 4, 6,

                                // Back
                                7, 3, 1,
                                7, 5, 3,

                                // Left
                                7, 0, 6,
                                0, 7, 1,

                                // Right
                                4, 2, 3,
                                3, 5, 4,

                                // Top
                                0, 1, 2,
                                2, 1, 3,

                                // Bottom
                                6, 4, 7,
                                7, 4, 5
                            };
    }
    public static class Face
    {

        public static FaceInstance Up = new FaceInstance(Matrix.Identity, new Vector3(), Vector3.Up);
        public static FaceInstance Down = new FaceInstance(Matrix.Identity, new Vector3(), Vector3.Down);
        public static FaceInstance Right = new FaceInstance(Matrix.Identity,  new Vector3(), Vector3.Right);
        public static FaceInstance Left = new FaceInstance(Matrix.Identity, new Vector3(), Vector3.Left);
        public static FaceInstance Back = new FaceInstance(Matrix.Identity,  new Vector3(), Vector3.Backward);
        public static FaceInstance Front = new FaceInstance(Matrix.Identity,  new Vector3(), Vector3.Forward);

        public readonly static FaceInstance Zero = new FaceInstance(new Matrix(),  new Vector3(), Vector3.Zero);
        private readonly static Vector3 Offset = new Vector3(0.5f, 0.5f, 0.5f);

        public readonly static Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0f);
        public readonly static Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);

        public readonly static Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0f);
        public readonly static Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);

        public readonly static Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0f);
        public readonly static Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);

        public readonly static Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0f);
        public readonly static Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);

        public readonly static Vertex[] Vertices = new Vertex[]
        {
            new Vertex( topLeftFront ), //0
            new Vertex( topRightFront ), //1

            new Vertex( bottomRightFront ), //2 
            new Vertex( bottomLeftFront ), // 3
            
        };
        public readonly static short[] Indices = new short[] {  
            // Front
            1, 2, 3, 
            3, 0, 1
        };
        public enum Facing
        {
            Backward = 0,
            Forward = 1,
            Left = 2,
            Right = 3,
            Up = 4,
            Down = 5
        }

        public static FaceInstance getInstancedFace(Facing facing, Vector3 position, Vector3 scale, Vector3 color)
        {
            FaceInstance i;
            switch (facing)
            {
                case Facing.Forward:
                    i = Front;
                    i.World = i.World * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset*scale) + position + (scale * (Vector3.Backward / 2)));
                    i.Normal = Vector3.Backward;
                    break;
                case Facing.Backward:
                    i = Back;
                    i.World = i.World * Matrix.CreateRotationY(-(float)Math.PI) * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset * scale) + position + (scale * (Vector3.Forward / 2)));
                    i.Normal = Vector3.Forward;
                    break;
                case Facing.Right:
                    i = Right;
                    i.World = i.World * Matrix.CreateRotationY((float)Math.PI / 2) * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset * scale) + position + (scale * (Vector3.Right / 2)));
                    i.Normal = Vector3.Right;
                    break;
                case Facing.Left:
                    i = Left;
                    i.World = i.World * Matrix.CreateRotationY(-(float)Math.PI / 2) * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset * scale) + position + (scale * (Vector3.Left / 2)));
                    i.Normal = Vector3.Left;
                    break;
                case Facing.Down:
                    i = Down;
                    i.World = i.World * Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset * scale) + position + (scale * (Vector3.Down / 2)));
                    i.Normal = Vector3.Down;
                    break;
                case Facing.Up:
                    i = Up;
                    i.World = i.World * Matrix.CreateRotationX(-(float)Math.PI / 2) * Matrix.CreateScale(scale) * Matrix.CreateTranslation((Offset * scale) + position + (scale * (Vector3.Up / 2)));
                    i.Normal = Vector3.Up;
                    break;
                default: return new FaceInstance();
            }

            i.Color = color;
            return i;
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
        //public static VertexPositionNormalColor[] getFace( Facing facing, Vector3 size, ref Voxel voxel )
        //{
        //    return getFace( facing, voxel.position, size, voxel.color);
        //}
        public static FaceInstance getInstancedFace(Facing facing, Vector3 Position)
        {
            return getInstancedFace(facing, Position);
        }

    //    public static VertexPositionNormalColor[] getFace( Facing facing, Vector3 Position, Vector3 size, Vector3 Color)
    //    {
           

    //        switch (facing)
    //        {
    //            case Facing.Forward:
    //                return new VertexPositionNormalColor[]
    //                {
    //                    // Front Surface
    //                    new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Backward, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Backward, Color), 
    //                    new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Backward, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Backward, Color), 
    //                    new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Backward, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Backward, Color),  
    //                };
    //            case Facing.Backward:
    //                return new VertexPositionNormalColor[]
    //                {
    //                    // Back Surface
    //                    new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Forward, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Forward, Color), 
    //                    new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Forward, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Forward, Color), 
    //                    new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Forward, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Forward, Color), 
    //                };
    //            case Facing.Right:
    //                return new VertexPositionNormalColor[]
    //                {
    //                    // Right Surface
    //                    new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Left, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Left, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Left, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Left, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Left, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Left, Color),
    //                };
    //            case Facing.Left:
    //                return new VertexPositionNormalColor[]
    //                {            
    //                    // Left Surface
    //                    new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Right, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Right, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Right, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Right, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Right, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Right, Color),
    //                };
    //            case Facing.Down:
    //                return new VertexPositionNormalColor[]
    //                {
    //                    // Bottom Surface
    //                    new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Down, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomRightBack*size)+Position, Vector3.Down, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Down, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftBack*size)+Position, Vector3.Down, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomLeftFront*size)+Position, Vector3.Down, Color),
    //                    new VertexPositionNormalColor((Voxel.bottomRightFront*size)+Position, Vector3.Down, Color),
                        
    //                };
    //            case Facing.Up:
    //                return new VertexPositionNormalColor[]
    //                {
    //                    // Top Surface
    //                    new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Up, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftBack*size)+Position, Vector3.Up, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Up, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightBack*size)+Position, Vector3.Up, Color),
    //                    new VertexPositionNormalColor((Voxel.topRightFront*size)+Position, Vector3.Up, Color),
    //                    new VertexPositionNormalColor((Voxel.topLeftFront*size)+Position, Vector3.Up, Color),
            
    //                };
    //            default: return new VertexPositionNormalColor[] { };
    //        }
    //    }
    }
}

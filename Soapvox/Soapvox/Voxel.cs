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

namespace Sandvox
{
    //public struct BasicVoxel
    //{
    //    public Color color;

    //    public BasicFace[] faces;
    //    public Vector3 position;
    //    public bool modified;

    //    public BasicVoxel( Color color, Vector3 position, Vector3 size)
    //    {
    //        this.color = color;
    //        this.position = position;
    //        this.modified = false;
    //        faces = getFaces(position, size, color);
    //    }
    //    public static bool operator ==(BasicVoxel a, BasicVoxel b)
    //    {
    //        if (a.color != b.color) return false;
    //        if (a.position != b.position) return false;
    //        return true;
    //    }
    //    public static bool operator !=(BasicVoxel a, BasicVoxel b)
    //    {
    //        if (a.color == b.color && a.position == b.position) return false;
    //        return true;
    //    }
    //    public static BasicFace[] getFaces( Vector3 position, Vector3 size, Color color)
    //    {
    //        BasicFace[] faces = new BasicFace[6];
    //        faces[(int)Face.Facing.Backward] = new BasicFace(Face.Facing.Backward, position, size, color);
    //        faces[(int)Face.Facing.Down] = new BasicFace(Face.Facing.Down, position, size, color);
    //        faces[(int)Face.Facing.Forward] = new BasicFace(Face.Facing.Forward, position, size, color);
    //        faces[(int)Face.Facing.Left] = new BasicFace(Face.Facing.Left, position, size, color);
    //        faces[(int)Face.Facing.Right] = new BasicFace(Face.Facing.Right, position, size, color);
    //        faces[(int)Face.Facing.Up] = new BasicFace(Face.Facing.Up, position, size, color);
    //        return faces;
    //    }
    //}
    //public enum VoxelColor
    //{
    //    Black,
    //    White,
    //    Red,
    //    Blue,
    //    Purple,
    //    Green,
    //    Orange,
    //    Yellow,
    //    Wheat,
    //    LightBlue

    //}

    //public class Voxel
    //{

    //    public static short[] indices = new short[] {
    //                     0,  1,  2,  // front face
    //                     1,  3,  2,
    //                     4,  5,  6,  // back face
    //                     6,  5,  7,
    //                     8,  9, 10,  // top face
    //                     8, 11,  9,
    //                     12, 13, 14, // bottom face
    //                     12, 14, 15,
    //                     16, 17, 18, // left face
    //                     19, 17, 16,
    //                     20, 21, 22, // right face
    //                     23, 20, 22  };
    //    //public static Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0.5f);
    //    //public static Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0.5f);
    //    //public static Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0.5f);
    //    //public static Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0.5f);
    //    //public static Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);
    //    //public static Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);
    //    //public static Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);
    //    //public static Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);
    //    public static Vector3 bottomRightFront = new Vector3(1f, 0f, 1f);
    //    public static Vector3 topRightFront = new Vector3(1f, 1f, 1f);
    //    public static Vector3 bottomLeftFront = new Vector3(0f, 0f, 1f);
    //    public static Vector3 topLeftFront = new Vector3(0f, 1f, 1f);
    //    public static Vector3 bottomRightBack = new Vector3(1f, 0f, 0f);
    //    public static Vector3 topRightBack = new Vector3(1f, 1f, 0f);
    //    public static Vector3 bottomLeftBack = new Vector3(0f, 0f, 0f);
    //    public static Vector3 topLeftBack = new Vector3(0f, 1f, 0f);
    //    public static Matrix voxelTranslation = Matrix.CreateTranslation(new Vector3(0.5f, 0.5f, 0.5f));
    //    public static VertexPositionNormalColor[] Default = new VertexPositionNormalColor[]
    //    {
            
    //        // Front Surface
    //        new VertexPositionNormalColor(bottomLeftFront, Vector3.Backward, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topLeftFront, Vector3.Backward, new Vector3(0,0,0)), 
    //        new VertexPositionNormalColor(bottomRightFront, Vector3.Backward, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightFront, Vector3.Backward, new Vector3(0,0,0)),  

    //        // Back Surface
    //        new VertexPositionNormalColor(bottomRightBack, Vector3.Forward, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightBack, Vector3.Forward, new Vector3(0,0,0)), 
    //        new VertexPositionNormalColor(bottomLeftBack, Vector3.Forward, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topLeftBack, Vector3.Forward, new Vector3(0,0,0)), 

    //        // Left Surface
    //        new VertexPositionNormalColor(bottomLeftBack, Vector3.Left, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topLeftBack, Vector3.Left, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(bottomLeftFront, Vector3.Left, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topLeftFront, Vector3.Left, new Vector3(0,0,0)),
            
    //        // Right Surface
    //        new VertexPositionNormalColor(bottomRightFront, Vector3.Right, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightFront, Vector3.Right, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(bottomRightBack, Vector3.Right, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightBack, Vector3.Right, new Vector3(0,0,0)),

    //        // Top Surface
    //        new VertexPositionNormalColor(topLeftFront, Vector3.Up, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topLeftBack, Vector3.Up, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightFront, Vector3.Up, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(topRightBack, Vector3.Up, new Vector3(0,0,0)),

    //        // Bottom Surface
    //        new VertexPositionNormalColor(bottomLeftBack, Vector3.Down, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(bottomLeftFront, Vector3.Down, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(bottomRightBack, Vector3.Down, new Vector3(0,0,0)),
    //        new VertexPositionNormalColor(bottomRightFront, Vector3.Down, new Vector3(0,0,0)),
             
            
    //    };

    //    public enum VoxelType
    //    {
    //        Open,
    //        Blank,
    //        Solid
    //    }

    //    public Vector3 color;

    //    protected VoxelType type;

    //    public Vector3 position;
    //    public bool modified = false;

    //    public Voxel(Vector3 color, Vector3 position, VoxelType type)
    //    {
    //        this.position = position;
    //        this.type = type;
    //        this.color = color;


    //    }
    //    public Voxel(Color color, Vector3 position, VoxelType type)
    //    {
    //        this.position = position;
    //        this.type = type;
    //        this.color = color.ToVector3();


    //    }
    //    public Voxel(Vector3 color, Vector3 position)
    //    {
    //        this.position = position;
    //        this.type = VoxelType.Solid;
    //        this.color = color;


    //    }
    //    public Voxel(Color color, Vector3 position)
    //    {
    //        this.position = position;
    //        this.type = VoxelType.Solid;
    //        this.color = color.ToVector3();

    //    }

    //    public void setColor(Color color)
    //    {

    //    }
    //    public void Update()
    //    {
    //    }


    //    public void Draw()
    //    {
    //    }
    //}
}

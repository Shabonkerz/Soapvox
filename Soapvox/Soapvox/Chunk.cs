using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandvox
{
    class Chunk
    {
        public static Vector3 DefaultSize = new Vector3(16,16,16);
        public List<VertexPositionNormalColor> vertices = new List<VertexPositionNormalColor>();
        VertexPositionNormalColor[] tmp;
        DynamicVertexBuffer VertexBuffer;
        public Vector3 Position;
        public Vector3 Size;
        Voxel[, ,] Voxels;
        public Matrix Translation = new Matrix();

        public Chunk()
        {
            Size = Chunk.DefaultSize;
            Voxels = new Voxel[(int)Size.X, (int)Size.Y, (int)Size.Z];
        }
        public Chunk(Vector3 Size)
        {
            this.Size = Size;
            Voxels = new Voxel[(int)Size.X, (int)Size.Y, (int)Size.Z];
        }
        public Chunk(Vector3 Size, Vector3 Position)
        {
            this.Size = Size;
            this.Position = Position;
            Voxels = new Voxel[(int)Size.X, (int)Size.Y, (int)Size.Z];
        }
        public void Add(Voxel voxel)
        {
            voxel.position -= this.Position;
            if (Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z] != null) return;
            //if (!inChunk(voxel.position)) return;
            //this.Remove(voxel);
            Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z] = voxel;

            if (((int)voxel.position.X + 1 < this.Size.X && Voxels[(int)voxel.position.X + 1, (int)voxel.position.Y, (int)voxel.position.Z] == null) || (int)voxel.position.X == this.Size.X - 1)
                vertices.AddRange(Face.getFace( Face.Facing.Right, voxel.position, voxel.color));

            if (((int)voxel.position.X - 1 >= 0 && Voxels[(int)voxel.position.X - 1, (int)voxel.position.Y, (int)voxel.position.Z] == null) || (int)voxel.position.X == 0)
                vertices.AddRange(Face.getFace( Face.Facing.Left, voxel.position, voxel.color));
            
            if (((int)voxel.position.Y + 1 < this.Size.Y && Voxels[(int)voxel.position.X, (int)voxel.position.Y + 1, (int)voxel.position.Z] == null) || (int)voxel.position.Y == this.Size.Y - 1)
                vertices.AddRange(Face.getFace( Face.Facing.Up, voxel.position, voxel.color));
            
            if (((int)voxel.position.Y - 1 >= 0 && Voxels[(int)voxel.position.X, (int)voxel.position.Y - 1, (int)voxel.position.Z] == null) || (int)voxel.position.Y == 0)
                vertices.AddRange(Face.getFace( Face.Facing.Down, voxel.position, voxel.color));
            
            if (((int)voxel.position.Z + 1 < this.Size.Z && Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z + 1] == null) || (int)voxel.position.Z == this.Size.Z - 1)
                vertices.AddRange(Face.getFace( Face.Facing.Forward, voxel.position, voxel.color));
            
            if (((int)voxel.position.Z - 1 >= 0 && Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z - 1] == null) || (int)voxel.position.Z == 0)
                vertices.AddRange(Face.getFace( Face.Facing.Backward, voxel.position, voxel.color));


            this.Build();

        }
        public void Add(Voxel[] voxels)
        {
            foreach (Voxel voxel in voxels)
                this.Add(voxel);
            
        
        }
        public void Add(Region region)
        {
            //foreach (Voxel voxel in region.Voxels)
               // Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z] = voxel;
        }
        public void Build()
        {
            if (vertices.Count == 0) return;
            VertexBuffer = new DynamicVertexBuffer(Game.graphics.GraphicsDevice, VertexPositionNormalColor.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalColor>(vertices.ToArray());
        }
        public void BuildVertices()
        {
            vertices = new List<VertexPositionNormalColor>();
            foreach (Voxel voxel in Voxels)
            {
                if (voxel != null)
                {
                    if (((int)voxel.position.X + 1 < this.Size.X && Voxels[(int)voxel.position.X + 1, (int)voxel.position.Y, (int)voxel.position.Z] == null ) || (int)voxel.position.X == this.Size.X - 1)
                        vertices.AddRange(Face.getFace(Face.Facing.Right, voxel.position, voxel.color));

                    if (((int)voxel.position.X - 1 >= 0 && Voxels[(int)voxel.position.X - 1, (int)voxel.position.Y, (int)voxel.position.Z] == null) || (int)voxel.position.X == 0 )
                        vertices.AddRange(Face.getFace(Face.Facing.Left, voxel.position, voxel.color));

                    if (((int)voxel.position.Y + 1 < this.Size.Y &&  Voxels[(int)voxel.position.X, (int)voxel.position.Y + 1, (int)voxel.position.Z] == null ) || (int)voxel.position.Y == this.Size.Y - 1)
                        vertices.AddRange(Face.getFace(Face.Facing.Up, voxel.position, voxel.color));

                    if (((int)voxel.position.Y - 1 >= 0 && Voxels[(int)voxel.position.X, (int)voxel.position.Y - 1, (int)voxel.position.Z] == null) || (int)voxel.position.Y == 0 )
                        vertices.AddRange(Face.getFace(Face.Facing.Down, voxel.position, voxel.color));

                    if (((int)voxel.position.Z + 1 < this.Size.Z &&  Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z + 1] == null ) || (int)voxel.position.Z == this.Size.Z - 1)
                        vertices.AddRange(Face.getFace(Face.Facing.Forward, voxel.position, voxel.color));

                    if (((int)voxel.position.Z - 1 >= 0 && Voxels[(int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z - 1] == null) || (int)voxel.position.Z == 0 )
                        vertices.AddRange(Face.getFace(Face.Facing.Backward, voxel.position, voxel.color));

                }
                
            }
            Build();
        }
        public int Search(Voxel voxel)
        {
            int j = 0;
            foreach (Voxel i in Voxels)
            {
                if (i == voxel) return j;
                j++;
            }
            return -1;
        }
        public void Remove( Vector3 v )
        {
            Remove((int)v.X, (int)v.Y, (int)v.Z);
        }

        public void Remove(Voxel voxel)
        {
            this.Remove((int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z);
        }
        public void Restore(int x, int y, int z)
        {
            if( z + 1< this.Size.Z && Voxels[x, y, z + 1] != null)
                insertFace(Face.Facing.Forward, ref Voxels[x, y, z + 1]);
            if( z > 0  && Voxels[x, y, z - 1] != null )
                insertFace(Face.Facing.Backward, ref Voxels[x, y, z - 1]);
            if (y + 1 < this.Size.Y && Voxels[x, y + 1, z] != null)
                insertFace(Face.Facing.Down, ref Voxels[x, y + 1, z]);
            if (y > 0 && Voxels[x, y - 1, z] != null)
                insertFace(Face.Facing.Up, ref Voxels[x, y - 1, z]);
            if (x + 1 < this.Size.X && Voxels[x + 1, y, z] != null)
                insertFace(Face.Facing.Left, ref Voxels[x + 1, y, z]);
            if (x > 0 && Voxels[x - 1, y, z] != null)
                insertFace(Face.Facing.Right, ref Voxels[x - 1, y, z]);
        }
        public List<VertexPositionNormalColor> getVisibleFaces( Voxel voxel )
        {
            return getVisibleFaces((int)voxel.position.X, (int)voxel.position.Y, (int)voxel.position.Z);
        }
        public List<VertexPositionNormalColor> getVisibleFaces(int x, int y, int z)
        {
            List<VertexPositionNormalColor> faces = new List<VertexPositionNormalColor>();

            if ((x + 1 < this.Size.X && Voxels[x + 1, y, z] == null) || x + 1 == this.Size.X)
                faces.AddRange(Face.getFace(Face.Facing.Right, Voxels[x, y, z].position, Voxels[x, y, z].color));

            if ((x - 1 >= 0 && Voxels[x - 1, y, z] == null) || x == 0)
                faces.AddRange(Face.getFace(Face.Facing.Left, Voxels[x, y, z].position, Voxels[x, y, z].color));

            if ((y + 1 < this.Size.Y && Voxels[x, y + 1, z] == null) || y + 1 == this.Size.Y)
                faces.AddRange(Face.getFace(Face.Facing.Up, Voxels[x, y, z].position, Voxels[x, y, z].color));

            if ((y - 1 >= 0 && Voxels[x, y - 1, z] == null) || y == 0)
                faces.AddRange(Face.getFace(Face.Facing.Down, Voxels[x, y, z].position, Voxels[x, y, z].color));

            if ((z + 1 < this.Size.Z && Voxels[x, y, z + 1] == null) || z + 1 == this.Size.Z)
                faces.AddRange(Face.getFace(Face.Facing.Forward, Voxels[x, y, z].position, Voxels[x, y, z].color));

            if ((z - 1 >= 0 && Voxels[x, y, z - 1] == null) || z == 0)
                faces.AddRange(Face.getFace(Face.Facing.Backward, Voxels[x, y, z].position, Voxels[x, y, z].color));

            return faces;
        }
        private bool inChunk(Vector3 v)
        {
            return inChunk((int)v.X, (int)v.Y, (int)v.Z);
        }
        private bool inChunk(int x, int y, int z)
        {
            return (x < this.Size.X + this.Position.X &&
                     x >= this.Position.X &&
                     y < this.Size.Y + this.Position.Y &&
                     y >= this.Position.Y &&
                     z < this.Size.Z + this.Position.Z &&
                     z >= this.Position.Z);
        }
        public void Remove(int x, int y, int z)
        {
            //if (!inChunk(x, y, z)) return;

            if (Voxels[x, y, z] == null) return;

            List<VertexPositionNormalColor> faces = new List<VertexPositionNormalColor>();

            faces = getVisibleFaces(x, y, z);
            
            // No visibles faces? Nothing to remove then! Not a multiple of six? Get outta here!
            if (faces == null || faces.Count % 6 > 0) return;

            // Let's find that face in the list...
            int i = getFaceIndex( ref faces );
            
            // Not found? Well let's bail then.
            if (i == -1) return;
            
            // Otherwise, let's remove them from the list.
            vertices.RemoveRange(i, faces.Count);

            //Restore(x, y, z);

            Voxels[x, y, z] = null;

        }
        public void insertFace( Face.Facing facing, ref Voxel voxel)
	    {
		    List<VertexPositionNormalColor> faces = getVisibleFaces( voxel );
		
		    // If empty, append the face to the end and exit;
		    if( faces.Count() == 0 )
		    {
			    VertexPositionNormalColor[] face = Face.getFace( facing, ref voxel );
			    vertices.AddRange( face );
			    return;
		    }

		    int index;

		    // If faces don't exist in the visible vertices, exit. Something's wrong if so, and this should never occur.
		    if( (index = getFaceIndex( ref faces ) ) == -1 )
			    return;

		
		    for( int i = 0; i < faces.Count() - 6; i = i + 6 )
		    {
			    if ( facing < Face.getFacing( faces.GetRange(i, 6)  ) )
			    {
				    vertices.InsertRange( index + i, Face.getFace( facing, ref voxel ) );
				    return;
			    }
		    }
            try
            {
                vertices.InsertRange(index + 30, Face.getFace(facing, ref voxel));
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

	    }
        public void Clear()
        {
            this.vertices.Clear();
            for( int i = 0; i < this.Size.X; i++ )
            {
                for (int j = 0; j < this.Size.Y; j++)
                {
                    for (int k = 0; k < this.Size.Z; k++)
                    {
                        Voxels[i,j,k] = null;
                    }
                }
            }
        }
        public int getFaceIndex(ref List<VertexPositionNormalColor> faces)
	    {
		    int index = 0;
            
            if (faces == null) return -1;

		    for( int i = 0; i < 20; i++ )
		    {
			
			    // Exit and return if vertices are not found.
                index = FindFirstFace( index, faces );
			    if(  index <= 0 || index > vertices.Count - 6 )
				    break;

			    // If one of the vertices doesn't match, we've found a different face. Retry search starting from last index.
			    if( vertices.ElementAt(index + 1).Position != faces[1].Position ||
                    vertices.ElementAt(index + 2).Position != faces[2].Position ||
                    vertices.ElementAt(index + 3).Position != faces[3].Position ||
                    vertices.ElementAt(index + 4).Position != faces[4].Position ||
                    vertices.ElementAt(index + 5).Position != faces[5].Position
				    )
					    continue;

			    return index;

		    }

		    // Couldn't find a face!
		    return -1;
	    }
        public int FindFirstFace(int index, List<VertexPositionNormalColor> faces )
        {
            try
            {
                return vertices.FindIndex(index, delegate(VertexPositionNormalColor v)
                {
                    return (v.Position == faces.ElementAt<VertexPositionNormalColor>(0).Position);
                });
            }
            catch (ArgumentOutOfRangeException e)
            {
                return -1;
            }
        }
        public void AddFace( Face.Facing facing, Vector3 position, Vector3 color)
        {
            tmp = Face.getFace(facing, position, color);
            if (tmp == null) return;
            for (int i = 0; i < 6; i++)
            {
                vertices.Add(tmp[i]);
            }
            tmp = null;
        }
        public void Update()
        {
            Translation = Matrix.CreateTranslation(Position);
        }
        public void Draw()
        {
            if (vertices.Count == 0) return;
            if (VertexBuffer == null) return;
            Game.graphics.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Game.effect.Parameters["worldViewProjectionMatrix"].SetValue(this.Translation * Game.WorldViewProjection);
            Game.effect.CurrentTechnique.Passes[0].Apply();
            Game.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.vertices.Count/3);
        }
    }
}

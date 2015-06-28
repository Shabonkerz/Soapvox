using System;
using System.Collections.Generic;
using System.Collections;
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
    class VoxelMap
    {
        const int ChunkHeight = 96;
        const int ChunkWidth = 96;
        const int ChunkDepth = 96;
        static int visibleRadius = 300;

        List<Region> regions = new List<Region>();
        
        Vector3 size;
        
        List<DynamicVoxel> voxels = new List<DynamicVoxel>();

        // List of all the chunks within the map.
        List<Chunk> chunks = new List<Chunk>();

        
        // An index array of all the visible chunks.
        List<Chunk> visibleChunks = new List<Chunk>();

        // Holds translation, roatation, scaling values.
        Matrix Translation;



        public VoxelMap( int x, int y, int z )
        {
            this.size = new Vector3( x, y, z );
        }
        public void addVoxel(Color color, DynamicVoxel.VoxelType type)
        {
            //this.voxels.Add( new DynamicVoxel(color, currentPosition, type) );
        }
        public void addVoxel(Color c, Vector3 position)
        {
            int i = SelectChunkByLocation(position);
            if (i == -1) return;
            chunks.ElementAt(i).Add(new Voxel(c, position));
        }
        public int SelectChunkByLocation( Vector3 v )
        {
            for( int i = 0; i < chunks.Count; i++ )
            {
                if ( v.X >= chunks.ElementAt(i).Position.X && v.X < chunks.ElementAt(i).Position.X + chunks.ElementAt(i).Size.X &&
                    v.Y >= chunks.ElementAt(i).Position.Y && v.Y < chunks.ElementAt(i).Position.Y + chunks.ElementAt(i).Size.Y &&
                    v.Z >= chunks.ElementAt(i).Position.Z && v.Z < chunks.ElementAt(i).Position.Z + chunks.ElementAt(i).Size.Z ) return i;

            }
            return -1;
        }
        public void addVoxel(Color color)
        {
            //if ((int)currentPosition.Z >= this.size.Z - 1 && (int)currentPosition.Y >= this.size.Y - 1 && (int)currentPosition.X >= this.size.X - 1) return;
            //this.voxels.Add(new DynamicVoxel(color, currentPosition));

        }
        public void addVoxel(DynamicVoxel voxel)
        {
            //if ((int)currentPosition.Z >= this.size.Z - 1 && (int)currentPosition.Y >= this.size.Y - 1 && (int)currentPosition.X >= this.size.X - 1) return;
            this.voxels.Add(voxel);

        }
        public void setVisibleChunks(int x, int y, int z)
        {
            int dist = 0;
            visibleChunks.Clear();
            foreach( Chunk c in chunks ) 
            {
                dist = (int)Vector3.Distance(new Vector3(x, y, z), (c.Position + c.Size)/ 2.0f );
                if (dist <= visibleRadius)
                {
                    visibleChunks.Add(c);
                }
            }
        }
        public void generateWaves()
        {
            Random r = new Random();
            int i = 0, j = 0, k = 0;
            for (i = 0; i < 96; i++)
            {
                j += r.Next(-1, 2);
                for (k = 0; k < 96; k++)
                {
                    addVoxel(Color.LightBlue, new Vector3(i, j, k));
                }
            }

            this.Build();
        }

        public void generatePlane( int width, int depth, int y, Color color )
        {
            int i = 0, k = 0;
            for (i = 0; i < depth; i++)
            {
                for (k = 0; k < width; k++)
                {
                    addVoxel( color, new Vector3(i, y, k));
                }
            }

            this.Build();
        }
        public void Build()
        {
            foreach (Chunk c in chunks)
                c.Build();
        }
        public void BuildVertices()
        {
            foreach (Chunk c in chunks)
                c.BuildVertices();
        }
        public void setVisibleChunks(Vector3 v)
        {
            setVisibleChunks((int)v.X, (int)v.Y, (int)v.Z);
        }
        public void addRegion(Region region)
        {
            regions.Add(region);
        }
        public void addChunk(ref Chunk chunk)
        {
            chunks.Add(chunk);
        }
        public void addChunk(Chunk chunk)
        {
            chunks.Add(chunk);
        }
        public void clearChunks()
        {
            chunks.Clear();
        }
        public void Draw()
        {

            //Game.effect.World = Matrix.Identity;
            /*
            for( int x = 0; x < size.X; x++ )
            {
                for (int y = 0; y < size.Y; y++)
                {
                    for (int z = 0; z < size.Z; z++)
                    {

                        Game.effect.World = Translation = Matrix.CreateTranslation(new Vector3(x,y,z));
                        if (voxels[x,y,z] != null) voxels[x,y,z].Draw();
                        Game.effect.World = Translation;
                    }
                }
            }*/
            foreach (DynamicVoxel voxel in voxels)
            {
                Translation = Game.WorldViewProjection;
                voxel.Draw();
                Game.WorldViewProjection = Translation;
            }
            foreach (Region region in regions)
            {
                Translation = Game.WorldViewProjection;
                region.Draw();
                Game.WorldViewProjection = Translation;
            }
            foreach (Chunk chunk in visibleChunks)
            {
                Translation = Game.WorldViewProjection;
                chunk.Draw();
                Game.WorldViewProjection = Translation;
            }
        }
        public void Update()
        {

            foreach (DynamicVoxel voxel in voxels)
            {
                voxel.Update();
            }
            foreach (Region region in regions)
            {
                region.Update();
            }
            foreach (Chunk chunk in visibleChunks)
            {
                chunk.Update();
            }
        }
        public int getVertexCount()
        {
            int i = 0;
            foreach (Chunk c in chunks)
            {
                i += c.vertices.Count;
            }
            return i;
        }
    }
}

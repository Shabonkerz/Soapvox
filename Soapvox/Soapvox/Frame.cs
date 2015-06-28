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
    class Frame
    {
        private List<Voxel> Voxels = new List<Voxel>();
        private List<Region> Regions = new List<Region>();
        //private DynamicVoxel Voxel;

        public Frame( Vector3 size )
        {
        }
        public void AddVoxel(Vector3 Position, Color color)
        {
            Voxels.Add( new Voxel( color, Position ) );
        }
        public void AddRegion(Vector3 Position, Color color, Vector3 Size)
        {
            Regions.Add( new Region( Size, color, Position ) );
        }
        public void Update()
        {
            foreach (Voxel voxel in Voxels)
            {
                voxel.Update();
            }
            foreach (Region region in Regions)
            {
                region.Update();
            }
        }
        public void Draw()
        {
            foreach (Voxel voxel in Voxels)
            {
                voxel.Draw();
            }
            foreach (Region region in Regions)
            {
                region.Draw();
            }
        }
    }
}

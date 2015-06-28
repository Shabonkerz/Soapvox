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
    class DynamicVoxel : Voxel
    {

        private Vector3 velocity;

        private bool shimmer = true;
        private Vector3 shimmerOffset = Vector3.Zero;
        private int shimmerRange;
        private int shimmerIncrement;

        public DynamicVoxel(Color color, Vector3 position, VoxelType type) : base( color, position, type )
        {
            this.velocity = Vector3.Zero;
        }
        public DynamicVoxel(Color color, Vector3 position) : base(color, position )
        {
            this.velocity = Vector3.Zero;
        }
        public void Shimmer(bool val)
        {
            this.shimmer = val;
        }
   
        public void ShimmerEffect()
        {
            if (!shimmer) return;
           //// this.shimmerIncrement = rand.Next(5, 20);
           // this.shimmerRange = rand.Next(5, 100);
            if (shimmerOffset.X >= shimmerRange || shimmerOffset.X <= -shimmerRange)
            {
                shimmerIncrement *= -1;
            }
            shimmerOffset += Vector3.One * shimmerIncrement;

        }
        public void Blink()
        {

        }
    }
}

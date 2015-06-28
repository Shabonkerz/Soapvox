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
    class BlinkEffect
    {
        public int speed;
        public int duration;
        public int count;
        public int R, G, B;
        public double lastUpdate;
        int tmpR, tmpG, tmpB;

        public BlinkEffect( int speed, int duration, int count, int R, int G, int B )
        {
            this.speed = speed;
            this.duration = duration;
            this.count = count;
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public void Blink( ref int R, ref int G, ref int B )
        {
            tmpR = R;
            tmpG = G;
            tmpB = B;

            R = this.R;
            G = this.G;
            B = this.B;

            this.R = tmpR;
            this.G = tmpG;
            this.B = tmpB;
        }
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - lastUpdate < speed) return;


            lastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}

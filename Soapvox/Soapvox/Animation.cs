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
    class Animation
    {
        private List<Frame> Frames = new List<Frame>();

        public Animation()
        {

        }
        public void addFrame( Frame frame )
        {
            this.Frames.Add(frame);
        }
        public void Update()
        {
            foreach (Frame frame in Frames)
            {
                frame.Update();
            }
        }
        public void Draw()
        {
            foreach (Frame frame in Frames)
            {
                frame.Draw();
            }
        }
    }
}

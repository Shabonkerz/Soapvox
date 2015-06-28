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
    class Player : Actor
    {
        private static Vector3 InitialPosition = new Vector3( 32, 1, 32 );
        private static Vector3 InitialFacing = Vector3.Backward;
        private static Vector3 Size = new Vector3(10, 25, 10);
        private static Animation Idle = new Animation();
        private static Frame IdleFrame = new Frame( Size );
        

        public Player( ) : base( InitialPosition, InitialFacing, Size )
        {
            IdleFrame.AddRegion(new Vector3(0,1,0), Color.White, new Vector3(3,4,3));
            IdleFrame.AddRegion(new Vector3(0,4,0), Color.White, new Vector3(1, 4, 1));
            Idle.addFrame( IdleFrame );
            Animations.Add(IdleAnimation, Idle);
            Speed = 1.0f;

        }
        public void Update()
        {
            base.Update();
        }
        public override void Draw()
        {
            Game.WorldViewProjection = Matrix.CreateTranslation(Position + new Vector3(0,jumpOffset,0));
            Animations[IdleAnimation].Draw();
            Game.WorldViewProjection = Matrix.Identity;
        }

    }
}

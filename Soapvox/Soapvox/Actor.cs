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
    class Actor
    {
        public Vector3 Position;
        protected Vector3 Facing;
        protected Vector3 Size;
        protected string IdleAnimation = "Idle";
        protected float Speed;
        protected Vector3 Movement;

        protected bool IsJumping = false;
        protected float jumpHeight = 10.0f;
        protected float jumpOffset = 0.0f;
        protected float jumpDirection = 1.0f;

        protected bool IsMoving = false;

        protected Dictionary<string, Animation> Animations =
            new Dictionary<string, Animation>();

        public Actor( Vector3 Position,  Vector3 Facing,  Vector3 Size )
        {
            this.Position = Position;
            this.Facing = Facing;
            this.Size = Size;
        }
        public void Move(Vector3 Direction)
        {
            if (IsJumping) return;
            Movement += Direction;
        }
        public void Jump()
        {
            IsJumping = true;
            jumpOffset = 0.0f;
        }
        public void Update()
        {
            if (IsJumping)
            {
                jumpOffset += jumpDirection;
                if (jumpOffset >= jumpHeight) jumpDirection *= -1;
                else if (jumpOffset < 0.0f)
                {
                    jumpDirection *= -1;
                    IsJumping = false;
                    jumpOffset = 0f;
                }
            }
            if (Movement != Vector3.Zero)
            {
                Position += Vector3.Normalize(Movement);
                Movement = Vector3.Zero;
            }
            Animations[IdleAnimation].Update();
        }
        public virtual void Draw()
        {
            Game.WorldViewProjection = Matrix.CreateTranslation(Position + new Vector3(0, jumpOffset, 0));
            Animations[IdleAnimation].Draw();
            Game.WorldViewProjection = Matrix.Identity;
        }
    }
}


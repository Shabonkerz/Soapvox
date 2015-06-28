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
    class Camera1
    {
        Vector3 Position = Vector3.Zero;
        Vector3 Up = Vector3.Up;
        Vector3 LookAt = Vector3.Zero;

        Vector3 originalPosition, originalUp, originalLookAt;

        Matrix Rotation;

        float xRotation, yRotation;

        float Speed = 1.0f;
        float rotationSpeed = 0.3f;

        public Camera1( Vector3 Position, Vector3 LookAt, Vector3 Up )
        {
             //= Matrix.CreateOrthographic(Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height, -1.0f, 1.0f);
            this.Position = originalPosition = Position;
            this.LookAt = originalLookAt = LookAt;
            this.Up = originalUp = Up;

            //xRotation = MathHelper.PiOver2;
            //yRotation = -MathHelper.Pi / 10.0f;
            Rotation = Matrix.Identity;
        }
        public Camera1()
        {

            //xRotation = MathHelper.PiOver2;
            //yRotation = -MathHelper.Pi / 10.0f;
            Rotation = Matrix.Identity;
        }
        public void Move( Vector3 direction )
        {
            direction = Vector3.Transform(direction, Rotation);
            Position += Speed * direction;
            this.Update();
        }
        public void Rotate()
        {

        }
        public void RotateX( float amount )
        {
            this.xRotation += amount * rotationSpeed;
            this.Update();
        }
        public void RotateY( float amount )
        {
            this.yRotation += amount * rotationSpeed;
            this.Update();
        }
        public void Update()
        {
            Rotation = Matrix.CreateRotationX(this.xRotation) * Matrix.CreateRotationY(this.yRotation);
            Vector3 RotatedTarget = Vector3.Transform(new Vector3(0, 0, -1), Rotation);
            Vector3 FinalTarget = Position + RotatedTarget;
            Game.view = Matrix.CreateLookAt(Position, FinalTarget, Vector3.Transform(Up, Rotation));
            
            /*Matrix cameraRotation = Matrix.CreateRotationX(this.xRotation) * Matrix.CreateRotationY(this.yRotation);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = Position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            Game.view = Matrix.CreateLookAt(Position, cameraFinalTarget, cameraRotatedUpVector);*/
        }
    }
}

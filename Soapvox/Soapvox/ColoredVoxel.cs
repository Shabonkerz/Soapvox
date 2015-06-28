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
    class VoxelTexture
    {
        public static void Initialize()
        {
        }
        static short[] indices = new short[] { 
            0, 1, 2, 2, 1, 3,   
            4, 5, 6, 6, 5, 7,
            8, 9, 10, 10, 9, 11, 
            12, 13, 14, 14, 13, 15, 
            16, 17, 18, 18, 17, 19,
            20, 21, 22, 22, 21, 23
        };
        public static Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0.5f);
        public static Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0.5f);
        public static Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0.5f);
        public static Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0.5f);
        public static Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);
        public static Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);
        public static Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);
        public static Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);

        private const float huewidth = 1/ (530.0f / 360.0f);

        public static Vector3 toHSL(int red, int green, int blue)
        {
            double h = 0, s = 0, l = 0;

            // normalize red, green, blue values

            double r = (double)red / 255.0;
            double g = (double)green / 255.0;
            double b = (double)blue / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            // hue

            if (max == min)
            {
                h = 0; // undefined

            }
            else if (max == r && g >= b)
            {
                h = 60.0 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60.0 * (g - b) / (max - min) + 360.0;
            }
            else if (max == g)
            {
                h = 60.0 * (b - r) / (max - min) + 120.0;
            }
            else if (max == b)
            {
                h = 60.0 * (r - g) / (max - min) + 240.0;
            }

            // luminance

            l = (max + min) / 2.0;

            // saturation

            if (l == 0 || max == min)
            {
                s = 0;
            }
            else if (0 < l && l <= 0.5)
            {
                s = (max - min) / (max + min);
            }
            else if (l > 0.5)
            {
                s = (max - min) / (2 - (max + min)); //(max-min > 0)?

            }

            return new Vector3(
                float.Parse(String.Format("{0:0.##}", h)),
                float.Parse(String.Format("{0:0.##}", s)),
                float.Parse(String.Format("{0:0.##}", l))
                );
        }
        public static Vector2 toTexCoords(int red, int green, int blue)
        {
            Vector3 HSL = VoxelTexture.toHSL(red, green, blue);

            return new Vector2( HSL.X/360.0f, HSL.Z  );
        }
        public static Vector2 toTexCoords(Vector3 HSL)
        {
            return new Vector2( HSL.X/360.0f, HSL.Z );
        }
    }
    
}

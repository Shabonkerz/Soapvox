using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sandvox
{
    class ColorOperations
    {
        public static Color SetContrast(Color color, double contrast)
        {

            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            double pR = color.R / 255.0;
            pR -= 0.5;
            pR *= contrast;
            pR += 0.5;
            pR *= 255;
            if (pR < 0) pR = 0;
            if (pR > 255) pR = 255;

            double pG = color.G / 255.0;
            pG -= 0.5;
            pG *= contrast;
            pG += 0.5;
            pG *= 255;
            if (pG < 0) pG = 0;
            if (pG > 255) pG = 255;

            double pB = color.B / 255.0;
            pB -= 0.5;
            pB *= contrast;
            pB += 0.5;
            pB *= 255;
            if (pB < 0) pB = 0;
            if (pB > 255) pB = 255;

            return Color.FromArgb((byte)pR, (byte)pR, (byte)pR);       
        }
    }
}

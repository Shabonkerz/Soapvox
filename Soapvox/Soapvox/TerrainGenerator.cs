using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sandvox
{
    class TerrainGenerator
    {
        public static Random rand = new Random(90);
        public static Octree<Volume> Generate( int width, int depth )
        {
            Octree<Volume> octree = new Octree<Volume>(new Volume(new Vector3(0,0,0), new Vector3(width, 64, depth), new Color()), Volume.AddHandler, Volume.RemoveHandler, Volume.SearchHandler, Volume.SetRootHandler, Volume.RemoveAllHandler);

            List<Vector3> coords = new List<Vector3>();
            int hills = rand.Next(10);
            int height = 0;
            for (int i = 0; i < hills; i++)
            {
                coords.Add( new Vector3( rand.Next(width), 0, rand.Next(depth)));
            }
            System.Drawing.Color c = System.Drawing.Color.LawnGreen;
            int r, g, b;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    octree.Add(
                                new Volume(
                                new Vector3(i, 0, j),
                                new Vector3(1, 1, 1),
                                new Color(c.R, c.G, c.B))
                                );
                }
            }
            foreach (Vector3 coord in coords)
            {
                for (int x = -10; x < 10; x++)
                {
                    for (int z = -10; z < 10; z++)
                    {
                        c = System.Drawing.Color.LawnGreen;
                        r = (int)(0.1f * (float)height * (int)c.R);
                        g = (int)c.G;
                        b = (int)(0.1f * (float)height * (int)c.B);

                        if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;
                        height = Math.Min(20 - Math.Abs(z), 20 - Math.Abs(x));
                        c = System.Drawing.Color.FromArgb(r, g, b);
                        octree.Add(
                            new Volume(
                            new Vector3(coord.X + x, height, coord.Z + z), 
                            new Vector3(1, 1, 1),
                            new Color(c.R, c.G, c.B))
                            );
                    }
                }
            }

            return octree;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Sandvox
{


    public class OctreeAdd
    {
        public int x;
        public int y;
        public int z;
        public Vector3 size;
        public Color color;
        public OctreeAdd(int x, int y, int z, Vector3 size, ref Color color)
        {
            this.x = x; this.y = y; this.z = z; this.size = size; this.color = color;
        }
    }
    public class OctreeRemove
    {
        public int x;
        public int y;
        public int z;
        public OctreeRemove(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }

    public class WorkerThread
    {
        private Thread thread;
        private Octree octree;
        private FaceBatch faceBatch;

        public WorkerThread()
        {
            thread = new Thread(new ThreadStart(Do));
            AddQueue = new Queue<OctreeAdd>();
            RemoveQueue = new Queue<OctreeRemove>();
            faceBatch = new FaceBatch<VertexPositionNormalColor>();
        }
        public void Start()
        {
            thread.Start();
        }
        public void Add(int x, int y, int z, Vector3 size, Color color)
        {
            AddQueue.Enqueue(new OctreeAdd(x, y, z, size, ref color));
        }
        public void Remove(int x, int y, int z)
        {
            RemoveQueue.Enqueue(new OctreeRemove(x, y, z));
        }
        public void Do()
        {
            OctreeAdd tmp;
            OctreeRemove remove;

            while (true)
            {
                System.Console.WriteLine("working it.");
                while (AddQueue.Count > 0)
                {
                    tmp = AddQueue.Dequeue();
                    if (tmp == null) continue;
                    if (tmp.x >= Size.X || tmp.y >= Size.Y || tmp.z >= Size.Z || tmp.x < 0 || tmp.y < 0 || tmp.z < 0) continue;
                    if (root == null)
                    {
                        this.root = new OctreeNode((int)Math.Max(Size.X, Math.Max(Size.Y, Size.Z)), 0, 0, 0, ref tmp.color);
                    }

                    root.Add(tmp.x, tmp.y, tmp.z, tmp.size, ref tmp.color);
                }


                while (RemoveQueue.Count > 0)
                {
                    remove = RemoveQueue.Dequeue();
                    if (root == null) continue;
                    root.Remove(remove.x, remove.y, remove.z);
                    UpdateNeighbors(remove.x, remove.y, remove.z);
                }

                faceBatch.Update();
            }
        }
        public void Stop()
        {
        }
    }
}
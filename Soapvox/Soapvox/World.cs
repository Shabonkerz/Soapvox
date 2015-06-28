using System;
using System.Collections;
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
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandvox
{


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

    public class World
    {
        private Thread thread;
        private Octree<WorldVolume> octree;
        public FaceBatch<VertexPositionNormalColor> faceBatch;
        private Queue AddQueue;
        private Queue RemoveQueue;
        private readonly object Locked = new object();


        private bool Started = false, Resumed = false, Suspended = false;
        
        public World( int size )
        {
            RemoveQueue = Queue.Synchronized(new Queue());
            AddQueue = Queue.Synchronized(new Queue());
            faceBatch = new FaceBatch<VertexPositionNormalColor>();
            octree = new Octree<WorldVolume>(new WorldVolume(new Vector3(-(int)Math.Pow(2.0f, size) / 2, -(int)Math.Pow(2.0f, size) / 2, -(int)Math.Pow(2.0f, size) / 2), 
                                                    new Vector3((int)Math.Pow(2.0f, size), (int)Math.Pow(2.0f, size), (int)Math.Pow(2.0f, size)), 
                                                    new Color()),
                                                    WorldVolume.AddHandler,
                                                    WorldVolume.RemoveHandler,
                                                    WorldVolume.SearchHandler,
                                                    WorldVolume.SetRootHandler, WorldVolume.RemoveAllHandler);
            thread = new Thread(new ThreadStart(Do));
        }
        public void Start()
        {
            Started = true;
            thread.Start();
        }
        public void Unload()
        {
            octree.RemoveAll();
            RemoveQueue.Clear();
            AddQueue.Clear();
            faceBatch.Update();
        }
        public void Add(Octree<Volume> octree)
        {
            List<Volume> list = octree.toList();
            foreach (Volume volume in list)
            {
                if( volume.Visible ) Add(volume);
            }
        }
        public void Add(Volume volume)
        {
            AddQueue.Enqueue(volume);
            lock (Locked)
            {
                Monitor.Pulse( Locked );
            }
        }
        public void Remove(Octree<Volume> octree)
        {
            List<Volume> list = octree.toList();
            foreach (Volume volume in list)
            {
                Remove(volume.Position);
            }
        }
        public void Remove(Vector3 position )
        {
            RemoveQueue.Enqueue(position);
            lock (Locked)
            {
                Monitor.Pulse(Locked);
            }
        }
        public void Do()
        {
            WorldVolume add;
            Vector3 remove;
            int addcount, removecount;
            while (true)
            {
                addcount = 0;
                removecount = 0;
                while (AddQueue.Count > 0 && addcount < 3000)
                {
                    add = new WorldVolume((Volume)AddQueue.Dequeue());
                    if (add == null) continue;
                    octree.Add( add );
                    addcount++;
                }
                while (RemoveQueue.Count > 0 && removecount < 3000)
                {
                    remove = (Vector3)RemoveQueue.Dequeue();
                    if (remove == null) continue;
                    Console.WriteLine("Removing " + remove);
                    octree.Remove( remove );
                    removecount++;
                }


                faceBatch.Update();
                lock (Locked)
                {
                    while (AddQueue.Count == 0 && RemoveQueue.Count == 0) Monitor.Wait(Locked);
                }
            }
            
        }
        public WorldVolume Search(int x, int y, int z)
        {
            return octree.Search( new Vector3(x, y, z));
        }
        public void Draw()
        {
            faceBatch.Draw();
        }
        public void Stop()
        {
            thread.Abort();
        }
        public string toJSON()
        {
            StringBuilder result = new StringBuilder();
            JsonTextWriter writer = new JsonTextWriter(new StringWriter(result));
            writer.Formatting = Formatting.Indented;
            writer.WriteStartArray();

            octree.Traverse((ref Octree<WorldVolume>.OctreeNode node) =>
            {
                
                if (node.data.Size != Vector3.One) return;

                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue((int)node.data.Position.X);
                writer.WritePropertyName("y");
                writer.WriteValue((int)node.data.Position.Y);
                writer.WritePropertyName("z");
                writer.WriteValue((int)node.data.Position.Z);
                writer.WritePropertyName("r");
                writer.WriteValue(node.data.color.R);
                writer.WritePropertyName("g");
                writer.WriteValue(node.data.color.G);
                writer.WritePropertyName("b");
                writer.WriteValue(node.data.color.B);
                writer.WriteEndObject();
            } );

            //root.toJSON(ref writer);
            
            writer.WriteEndArray();

            return result.ToString();

        }
        public byte[] toBytes()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter b = new BinaryWriter(buffer);

            octree.Traverse((ref Octree<WorldVolume>.OctreeNode node) =>
            {

                if (node.data.Size != Vector3.One) return;

                b.Write((int)node.data.Position.X);
                b.Write((int)node.data.Position.Y);
                b.Write((int)node.data.Position.Z);
                b.Write(node.data.color.R);
                b.Write(node.data.color.G);
                b.Write(node.data.color.B);
            });
            
            return buffer.ToArray();
        }
        public void loadFromURL(string url, int x, int y, int z)
        {
            WebClient wc = new WebClient();
            byte[] data = wc.DownloadData(url);
            MemoryStream ms = new MemoryStream(data);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(ms);
            System.Drawing.Color c;


            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    c = image.GetPixel(i, j);
                    Sandvox.Game.world.Add(new Volume(new Vector3(i + x, image.Height - j + y, z), new Vector3(1, 1, 1), new Color(c.R, c.G, c.B)));
                }
            }

            ms.Close();
            wc.Dispose();

        }
        public void loadFromFile(string filename) 
        {
            int x = 0;
            int y = 0;
            int z = 0;
            byte r = 0;
            byte g = 0;
            byte b = 0;

            //FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] buffer;
            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    int pos = 0;

                    int length = (int)br.BaseStream.Length;
                    int size = 1020;
                    //byte[] bytes;
                    while (pos < length)
                    {
                        if (length - pos < 1020)
                            size = length - pos;

                        buffer = br.ReadBytes(size);

                        for (int i = 0; i < size; i += 15)
                        {
                            byte[] xbytes = { buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3] };


                            x = BitConverter.ToInt32(xbytes, 0);


                            byte[] ybytes = { buffer[i + 4], buffer[i + 5], buffer[i + 6], buffer[i + 7] };


                            y = BitConverter.ToInt32(ybytes, 0);

                            byte[] zbytes = { buffer[i + 8], buffer[i + 9], buffer[i + 10], buffer[i + 11] };


                            z = BitConverter.ToInt32(zbytes, 0);

                            r = buffer[i + 12];
                            g = buffer[i + 13];
                            b = buffer[i + 14];

                            System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
                            Add( new Volume(new Vector3(x, y, z), new Vector3(1f, 1f, 1f), new Color(c.R, c.G, c.B, c.A)));
                        }

                        pos += size;
                    }
                }
            

        }
        public void loadFromJSONFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {


                JsonTextReader jtr = new JsonTextReader(sr);
                //JObject nodes = JObject.Parse(sr.ReadToEnd());
                Volume node = new Volume();
                Hashtable jobject = new Hashtable();
                string property = "";
                while (jtr.Read())
                {
                    switch (jtr.TokenType)
                    {
                        case JsonToken.StartObject:
                            Console.Write("Start object: ");
                            break;
                        case JsonToken.StartArray:
                            break;
                        case JsonToken.EndArray:
                            break;
                        case JsonToken.PropertyName:
                            property = jtr.Value.ToString();
                            break;
                        case JsonToken.EndObject:
                            if (jobject.Count > 0)
                            {
                                System.Drawing.Color c = System.Drawing.Color.FromArgb(int.Parse(jobject["r"].ToString()), int.Parse(jobject["g"].ToString()), int.Parse(jobject["b"].ToString()));
                                Add(
                                    new Volume(
                                        new Vector3(int.Parse(jobject["x"].ToString()), int.Parse(jobject["y"].ToString()), int.Parse(jobject["z"].ToString())), 
                                        new Vector3(1f, 1f, 1f), 
                                        new Color(c.R, c.G, c.B, c.A))
                                        );
                                jobject.Clear();
                            }
                            break;
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                            jobject.Add(property, jtr.Value);
                            break;
                    }
                }

            }

        }
    }
}
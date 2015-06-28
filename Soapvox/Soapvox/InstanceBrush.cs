using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using System.Threading;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandvox
{
    public class InstanceBrush
    {
        public Octree<Volume> octree;
        List<Volume> list;

        public void saveToFile(string filename)
        {

        }
        public void saveToJSONFile(string filename)
        {
            //using (StreamWriter outfile = new StreamWriter(filename))
            //{
            //    outfile.Write(octree.toJSON());
            //}
        }
        public void loadFromJSONFile(string filename)
        {
            list.Clear();
            using (StreamReader sr = new StreamReader(filename))
            {
                JsonTextReader jtr = new JsonTextReader(sr);

                Hashtable jobject = new Hashtable();

                string property = "";
                int width = 5, height = 5, depth = 5;
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
                                octree.Add( new Volume( 
                                                        new Vector3(int.Parse(jobject["x"].ToString()), int.Parse(jobject["y"].ToString()), int.Parse(jobject["z"].ToString())), 
                                                        new Vector3(1f, 1f, 1f), 
                                                        new Color(c.R, c.G, c.B, c.A)));
                                jobject.Clear();
                            }
                            break;
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                            if (property == "width")
                            {
                                width = int.Parse(jtr.Value.ToString());
                            }
                            else if (property == "height")
                            {
                                height = int.Parse(jtr.Value.ToString());
                            }
                            else if (property == "depth")
                            {
                                depth = int.Parse(jtr.Value.ToString());
                                octree = new Octree<Volume>( new Volume( new Vector3(0,0,0), new Vector3(width, height, depth), new Color() ), Volume.AddHandler, Volume.RemoveHandler, Volume.SearchHandler, Volume.SetRootHandler, Volume.RemoveAllHandler);
                            }
                            else
                            {
                                jobject.Add(property, jtr.Value);
                            }
                            break;
                    }
                }

            }
        }
        public void loadFromFile(string filename)
        {
            int x = 0;
            int y = 0;
            int z = 0;
            byte r = 0;
            byte g = 0;
            byte b = 0;

            byte[] buffer;
            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {

                int width = br.ReadInt32();

                int height = br.ReadInt32();

                int depth = br.ReadInt32();


                int pos = 12;

                int length = (int)br.BaseStream.Length;
                int size = 1020;


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
                        octree.Add( new Volume( new Vector3(x, y, z), new Vector3(1f, 1f, 1f), new Color(c.R, c.G, c.B, c.A)));
                    }

                    pos += size;
                }
            }
        }
        public List<Volume> toList()
        {
            if (list.Count > 0) return list;

            list = octree.toList();
            return list;
        }
        public InstanceBrush(string filename)
        {

            list = new List<Volume>();
            this.loadFromJSONFile(filename);
        }
    }
}

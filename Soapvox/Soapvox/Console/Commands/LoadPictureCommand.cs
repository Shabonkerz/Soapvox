using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandvox;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class LoadPictureCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: loadpic <url> <x> <y> <z>";
        string Output;
        public LoadPictureCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("URI", "uri", "")
            };
        }
        public bool Execute(string[] arguments)
        {

            this.Arguments = arguments;

            return Execute();
        }
        public bool Execute()
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(this.Arguments[1]);


            if (this.Arguments.Length >= Parameters.Length)
            {
                System.Drawing.Color c;

                int x = int.Parse(this.Arguments[2]);
                int y = int.Parse(this.Arguments[3]);
                int z = int.Parse(this.Arguments[4]);

                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        c = image.GetPixel(i, j);
                        Sandvox.BlockGame.world.Add( new Volume( new Vector3(i + x, image.Height - j + y, z), new Vector3(1, 1, 1), new Color(c.R, c.G, c.B)));
                    }
                }
                Output += this.Arguments[1] + "(" + image.Width + "," + image.Height + ") summoned at (" + this.Arguments[2] + "," + this.Arguments[3] + "," + this.Arguments[4] + ")";

                return true;
            }
            else
                Output = "Invalid parameters.\n" + HelpMessage;
            return false;
        }
        public bool Undoable()
        {
            return true;
        }
        public void Unexecute()
        {

        }
        public string getOutput()
        {
            return Output;
        }

    }
}

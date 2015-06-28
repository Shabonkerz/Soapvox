using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sandvox;
using SandvoxConsole;
using Microsoft.Xna.Framework;

namespace SandvoxConsole.Commands
{
    class AddCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: add <x> <y> <z> <x size> <y size> <z size> <R(0-255)> <G(0-255)> <B(0-255)>";
        string Output;
        World world;
        public AddCommand( ref World world )
        {
            this.Output = "Invalid parameters.\n" + HelpMessage;
            this.world = world;
            Parameters = new Parameter[] { 
                new Parameter("x", "int", "<x> must be a valid non-negative integer. e.g. 24."),
                new Parameter("y", "int", "<y> must be a valid non-negative integer. e.g. 89."),
                new Parameter("z", "int", "<z> must be a valid non-negative integer. e.g. 190."),
                new Parameter("size x", "int", ""),
                new Parameter("size y", "int", ""),
                new Parameter("size z", "int", ""),
                new Parameter("r", "color", ""),
                new Parameter("g", "color", ""),
                new Parameter("b", "color", ""),
            };
        }
        public bool Execute(string[] arguments)
        {
           
            this.Arguments = arguments;

            return Execute();
        }
        private bool IsValid()
        {
            if (this.Arguments.Length < Parameters.Length) return false;

            if (int.Parse(this.Arguments[7]) > 255 || int.Parse(this.Arguments[7]) < 0 ||
            int.Parse(this.Arguments[8]) > 255 || int.Parse(this.Arguments[8]) < 0 ||
            int.Parse(this.Arguments[9]) > 255 || int.Parse(this.Arguments[9]) < 0)
                return false;

            return true;
        }
        public bool Undoable()
        {
            return true;
        }
        public bool Execute()
        {
            if (!IsValid()) return false;

            System.Drawing.Color c = System.Drawing.Color.FromArgb(int.Parse(this.Arguments[7]), int.Parse(this.Arguments[8]), int.Parse(this.Arguments[9]));

            world.Add(new Volume(new Vector3(int.Parse(this.Arguments[1]), int.Parse(this.Arguments[2]), int.Parse(this.Arguments[3])),
                                    new Vector3(int.Parse(this.Arguments[4]), int.Parse(this.Arguments[5]), int.Parse(this.Arguments[6])),
                                    new Color(c.R, c.G, c.B, c.A)));
                
            Output = "Added block at (" + this.Arguments[1] + "," + this.Arguments[2] + "," + this.Arguments[3] + ") of size(" + this.Arguments[4] + "," + this.Arguments[5] + "," + this.Arguments[6] + ")";
            return true;
            
        }

        public void Unexecute()
        {
            System.Drawing.Color c = System.Drawing.Color.FromArgb(int.Parse(this.Arguments[7]), int.Parse(this.Arguments[8]), int.Parse(this.Arguments[9]));
            world.Remove( new Volume( new Vector3(int.Parse(this.Arguments[1]), int.Parse(this.Arguments[2]), int.Parse(this.Arguments[3])),
                                        new Vector3(int.Parse(this.Arguments[4]), int.Parse(this.Arguments[5]), int.Parse(this.Arguments[6])),
                                        new Color(c.R, c.G, c.B, c.A)));
            Output = "Undo: " + Output;
        }
        public string getOutput()
        {
            return Output;
        }
    }


    
    
}

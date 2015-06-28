using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Sandvox;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class LoadPictureFromURLCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: loadpicfromurl <url>";
        string Output;
        Octree<Volume> octree;
        public LoadPictureFromURLCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("filename", "string", ""),
                new Parameter("x", "int", ""),
                new Parameter("y", "int", ""),
                new Parameter("z", "int", "")
            };
        }
        public bool Execute(string[] arguments)
        {

            this.Arguments = arguments;

            return Execute();
        }
        public bool Execute()
        {
            if (this.Arguments.Length >= Parameters.Length)
            {
                //octree = new Octree<Volume>();
                Sandvox.BlockGame.world.loadFromURL(this.Arguments[1], int.Parse(this.Arguments[2]), int.Parse(this.Arguments[3]), int.Parse(this.Arguments[4]));
                Output = this.Arguments[1] + " loaded.";
                return true;
            }
            else
                Output = "Invalid parameters.\n" + HelpMessage;
            return false;
        }
        public void Unexecute()
        {

        }
        public bool Undoable()
        {
            return true;
        }
        public string getOutput()
        {
            return Output;
        }
    }

}

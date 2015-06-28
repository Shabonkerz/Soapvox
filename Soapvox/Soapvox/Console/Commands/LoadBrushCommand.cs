using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class LoadBrushCommand : ICommand
      {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: loadbrush <filename>";
        string Output;
        public LoadBrushCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("filename", "int", "")
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
                try
                {
                    Sandvox.BlockGame.brush.loadFromJSONFile(this.Arguments[1]);
                    Output = "Loaded brush " + this.Arguments[1] + ".";
                }
                catch (FileNotFoundException)
                {
                    Output = "File not found.";
                }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class SetBrushCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: setbrush <size x> <size y> <size z> <r> <g> <b>";
        string Output;
        public SetBrushCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("x", "int", ""),
                new Parameter("y", "int", ""),
                new Parameter("z", "int", ""),
                new Parameter("r", "int", ""),
                new Parameter("g", "int", ""),
                new Parameter("b", "int", "")
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
                System.Drawing.Color c = System.Drawing.Color.FromArgb(int.Parse(this.Arguments[4]), int.Parse(this.Arguments[5]), int.Parse(this.Arguments[6]));
                Sandvox.BlockGame.setBrushColor(c);
                Sandvox.BlockGame.setBrushSize(int.Parse(this.Arguments[1]), int.Parse(this.Arguments[2]), int.Parse(this.Arguments[3]));

                Output = "Brush set.";
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

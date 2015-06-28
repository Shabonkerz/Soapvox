using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class SaveCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: save <filename>";
        string Output;
        public SaveCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("filename", "string", "")
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
                BinaryWriter br = new BinaryWriter(File.Open(this.Arguments[1], FileMode.Create));
                br.Write(Sandvox.BlockGame.world.toBytes());
                br.Close();
                Output = this.Arguments[1] + " saved.";
                return true;
            }
            else
                Output = "Invalid parameters.\n" + HelpMessage;
            return false;
        }
        public bool Undoable()
        {
            return false;
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

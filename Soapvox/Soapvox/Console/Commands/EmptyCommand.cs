using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandvoxConsole;

namespace SandvoxConsole.Commands
{
    class EmptyCommand : ICommand
    {
        string Output = "No command specified.";
        public EmptyCommand()
        {
            
        }

        public bool Execute(string[] arguments)
        {

            return Execute();
        }
        public bool Undoable()
        {
            return false;
        }
        public bool Execute()
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

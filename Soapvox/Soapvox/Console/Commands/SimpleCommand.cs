using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandvoxConsole.Commands
{
    class SimpleCommand : ICommand
    {
        string Output;
        Action action;
        public SimpleCommand( Action action )
        {
            this.action = action;
        }
        public bool Execute(string[] arguments)
        {
            Output = "Unable to execute.";
            return Execute();
        }
        public bool Execute()
        {
            action();
            Output = "Executed.";
            return true;
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

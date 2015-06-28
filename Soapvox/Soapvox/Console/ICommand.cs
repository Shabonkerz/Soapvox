using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandvoxConsole
{
    class Parameter
    {
        string Type;
        string ErrorMessage;
        string Name;

        public Parameter( string Name, string Type, string ErrorMessage )
        {
            this.Name = Name;
            this.Type = Type;
            this.ErrorMessage = ErrorMessage;
        }
    }
    interface ICommand
    {
        bool Execute(string[] arguments);
        bool Execute();
        void Unexecute();
        string getOutput();
        bool Undoable();
    }
}

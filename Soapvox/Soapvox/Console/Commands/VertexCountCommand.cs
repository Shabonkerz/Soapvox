using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SandvoxConsole;

namespace SandvoxConsole.Commands

{
    class VertexCountCommand : ICommand
    {
        string Output;
        public VertexCountCommand()
        {

        }
        public bool Execute(string[] arguments)
        {

            return Execute();
        }
        public bool Execute()
        {
            Output = "Total vertices: " + Sandvox.BlockGame.world.faceBatch.Count();
            return false;
        }
        public void Unexecute()
        {

        }
        public bool Undoable()
        {
            return false;
        }
        public string getOutput()
        {
            return Output;
        }
    }
}

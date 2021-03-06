﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SandvoxConsole;

namespace SandvoxConsole.Commands

{
    class LoadCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: load <filename>";
        string Output;
        public LoadCommand()
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
        public bool Undoable()
        {
            return true;
        }
        public bool Execute()
        {
            if (this.Arguments.Length >= Parameters.Length)
            {
                try
                {
                    Sandvox.BlockGame.world.loadFromFile(this.Arguments[1]);
                    Output = this.Arguments[1] + " loaded.";
                    return true;
                }
                catch (FileNotFoundException e)
                {
                    Output = "File not found.";
                    return false;
                }
            }
            else
                Output = "Invalid parameters.\n" + HelpMessage;
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

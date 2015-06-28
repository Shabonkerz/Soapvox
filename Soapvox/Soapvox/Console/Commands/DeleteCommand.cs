using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandvox;
namespace SandvoxConsole.Commands
{
    /// <summary>
    /// Does stuff.
    /// </summary>
    class DeleteCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: ";
        string Output;

        /// <summary>
        /// Constructs a new command.
        /// </summary>
        public DeleteCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("x", "int", "")
            };
            // Set output for failure.
            Output = "Invalid parameters.\n" + HelpMessage;
        }
        private bool IsValid()
        {
            if (this.Arguments.Length < Parameters.Length) return false;
            
            return true;
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="arguments">The string array of arguments for the command.</param>
        /// <returns>Returns true if successful, otherwise false.</returns>
        
        public bool Execute(string[] arguments)
        {

            this.Arguments = arguments;

            return Execute();
        }
        public bool Undoable()
        {
            return false;
        }
        public bool Execute()
        {
            if (!IsValid()) return false;
            
            Sandvox.BlockGame.world.Unload();
            // Set output for success.
            Output = "World unloaded successfully.";
            return true;
        }
        /// <summary>
        /// Unexecute the command.
        /// </summary>
        public void Unexecute()
        {

        }

        /// <summary>
        /// Get the result of command execution.
        /// </summary>
        /// <returns>A message pertaining to the success or failure of the command.</returns>
        public string getOutput()
        {
            return Output;
        }
    }
}

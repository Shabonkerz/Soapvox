using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sandvox;

namespace SandvoxConsole.Commands
{
    /// <summary>
    /// Does stuff.
    /// </summary>
    class RemoveCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: ";
        string Output;

        /// <summary>
        /// Constructs a new command.
        /// </summary>
        public RemoveCommand()
        {
            Parameters = new Parameter[] { 
                new Parameter("x", "int", "")
            };
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
        public bool Execute()
        {
            if (this.Arguments.Length >= Parameters.Length)
            {
                Vector3 tmp = new Vector3( int.Parse(Arguments[1]) , int.Parse(Arguments[2]), int.Parse(Arguments[3]));

                // Do something here.
                Sandvox.BlockGame.world.Remove(new Volume( new Vector3(int.Parse(Arguments[1]) , int.Parse(Arguments[2]), int.Parse(Arguments[3])), Vector3.One, new Color() ));
                // Set output for success.
                Output = "Removed " + tmp;
                return true;
            }
            else

                // Set output for failure.
                Output = "Invalid parameters.\n" + HelpMessage;
            return false;
        }
        public bool Undoable()
        {
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

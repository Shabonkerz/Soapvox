using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandvoxConsole.Commands
{
    /// <summary>
    /// Does stuff.
    /// </summary>
    class TemplateCommand : ICommand
    {
        Parameter[] Parameters;
        string[] Arguments;
        string HelpMessage = "Usage: ";
        string Output;

        /// <summary>
        /// Constructs a new command.
        /// </summary>
        public TemplateCommand()
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
            if (this.Arguments.Length >= Parameters.Length)
            {
                // Do something here.

                // Set output for success.
                Output = "Some kind of output.";
                return true;
            }
            else

                // Set output for failure.
                Output = "Invalid parameters.\n" + HelpMessage;

            return false;
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

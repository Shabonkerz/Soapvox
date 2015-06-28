using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandvoxConsole.Commands
{
    /// <summary>
    /// Does stuff.
    /// </summary>
    class MacroCommand : ICommand
    {
        string HelpMessage = "Usage: ";
        string Output;
        List<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// Constructs a new command.
        /// </summary>
        public MacroCommand()
        {
            
            Output = "Invalid parameters.\n" + HelpMessage;

        }
        public void AddCommand(ICommand command)
        {
            commands.Add(command);
        }
        public void RemoveCommand( int index )
        {
            commands.RemoveAt(index);
        }
        public bool Undoable()
        {
            return true;
        }
        private bool IsValid()
        {


            return true;
        }
        public bool Execute(string[] arguments)
        {


            return Execute();
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="arguments">The string array of arguments for the command.</param>
        /// <returns>Returns true if successful, otherwise false.</returns>
        public bool Execute()
        {
            if ( !IsValid() ) return false;

            // Do something here.
            foreach (ICommand command in commands)
            {
                command.Execute();
                Output += command.getOutput() + "\n";
            }

            // Set output for success.
            return true;
            
        }

        /// <summary>
        /// Unexecute the command.
        /// </summary>
        public void Unexecute()
        {
            for (int i = commands.Count-1; i >= 0; i--) 
                commands[i].Execute();
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

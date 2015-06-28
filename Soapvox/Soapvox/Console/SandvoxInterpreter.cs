using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SandvoxConsole.Commands;

namespace SandvoxConsole
{
    public class SandvoxInterpreter : DrawableGameComponent
    {
        const string Prompt = ">>> ";
        const string PromptCont = "... ";
        public XnaConsoleComponent Console;
        Stack<ICommand> Commands = new Stack<ICommand>();
        Stack<ICommand> UndoneCommands = new Stack<ICommand>();

        delegate ICommand CommandHandler();

        Dictionary<string, CommandHandler> CommandHandlers;

        /// <summary>
        /// Creates a new SandvoxInterpreter
        /// </summary>
        public SandvoxInterpreter(Game game, SpriteFont font)
            : base((Game)game)
        {

            Console = new XnaConsoleComponent(game, font);
            game.Components.Add(Console);
            Console.Prompt(Prompt, Execute);
            AddGlobal("Console", Console);

            CommandHandlers = new Dictionary<string, CommandHandler>();
            AddCommand("add",               () => { return new AddCommand(ref Sandvox.BlockGame.world ); });
            AddCommand("loadpic",           () => { return new LoadPictureCommand(); });
            AddCommand("vertexcount",       () => { return new VertexCountCommand(); });
            AddCommand("save",              () => { return new SaveCommand(); });
            AddCommand("load",              () => { return new LoadCommand(); });
            AddCommand("setbrush",          () => { return new SetBrushCommand(); });
            AddCommand("loadpicfromurl",    () => { return new LoadPictureFromURLCommand(); });
            AddCommand("loadbrush",         () => { return new LoadBrushCommand(); });
            AddCommand("unload",            () => { return new DeleteCommand(); });
            AddCommand("remove",            () => { return new RemoveCommand(); });
            AddCommand("generateterrain",   () => { return new GenerateTerrainCommand(); });
            AddCommand("loadfromjson",      () => { return new LoadFromJSONCommand(); });
        }
        private void AddCommand( string key, CommandHandler value)
        {
            CommandHandlers.Add(key, value);
        }
        /// <summary>
        /// Executes commands from the console.
        /// </summary>
        /// <param name="input">The string containing the command and parameters.</param>
        /// <returns>Returns the execution results or error messages.</returns>
        public void Execute(string input)
        {
            if (input == "undo")
            {
                Unexecute();
                return;
            }
            else if (input == "redo")
            {
                Reexecute();
                return;
            }

            bool UnrecognizedCommand = false;
            string[] arguments = input.Split(new Char[] { ' ' });
            ICommand cmd;

            if (!CommandHandlers.ContainsKey(arguments[0]))
            {
                cmd = new EmptyCommand();
                UnrecognizedCommand = true;
            }
            else
            {
                cmd = CommandHandlers[arguments[0]]();
            }

            if (!UnrecognizedCommand) UndoneCommands.Clear();
            if (cmd.Execute(arguments)) Commands.Push(cmd);
            Console.WriteLine(cmd.getOutput());
            Console.Prompt(Prompt, Execute);

        }
        public void Unexecute()
        {
            if (Commands.Count == 0) return;

            ICommand tmp;
            do
            {
                tmp = Commands.Pop();
            }
            while (tmp.Undoable());

            tmp.Unexecute();
            UndoneCommands.Push(tmp);
            Console.WriteLine(tmp.getOutput());
            Console.Prompt(Prompt, Execute);
        }
        public void Reexecute()
        {
            if (UndoneCommands.Count == 0) return;

            ICommand tmp = UndoneCommands.Pop();
            tmp.Execute();
            Commands.Push(tmp);
            Console.WriteLine(tmp.getOutput());
            Console.Prompt(Prompt, Execute);
        }
        /// <summary>
        /// Adds a global variable to the environment of the interpreter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddGlobal(string name, object value)
        {
            //PythonEngine.Globals.Add(name, value);
        }
    }
}

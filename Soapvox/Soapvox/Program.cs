using System;

namespace Sandvox
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (BlockGame game = new BlockGame())
            {
                game.Run();
            }
        }
    }
#endif
}


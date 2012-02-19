using System;

namespace OscilloscopeEmulator
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            int width = 600, height = 600;
            bool inv = false;
            if (args.Length >= 1)
            {
                int offset = 0;
                if (args[0] == "white" || args[0] == "black")
                {
                    inv = args[0] == "white";
                    offset++;
                }

                if (args.Length >= 2)
                {
                    width = height = int.Parse(args[1]);

                    if (args.Length >= 3)
                        height = int.Parse(args[2]);
                }
            }

            using (Game1 game = new Game1(inv, width, height))
            {
                game.Run();
            }
        }
    }
#endif
}


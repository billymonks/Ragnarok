using System;

namespace wickedcrush
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WinGame game = new WinGame())
            {
                game.Run();
            }
        }
    }
#endif
}


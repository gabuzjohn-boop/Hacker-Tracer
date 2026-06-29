using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer
{
    public class Game
    {
        public void Run()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "GHOSTNET TERMINAL v2.7";

            Terminal.Boot();

            
        }
    }
}

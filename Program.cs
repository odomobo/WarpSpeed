using System;
using Orbits;

namespace WarpSpeed
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(1600, 1200, 144);
            game.Run();
        }
    }
}

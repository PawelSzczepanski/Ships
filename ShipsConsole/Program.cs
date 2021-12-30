using System;
using ScreenGenerator;
using Ships;
using ShipsEngine;

namespace ShipsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int rowSize = 10;
            int[] shipsArray = { 5, 4, 4, 3, 3, 2 };

            BoardConstraints boardRules = new BoardConstraints(rowSize, shipsArray);
            IScreenGenerator generator = new ScreenGenerator.ScreenGenerator(boardRules);
            IEngine engine = new Engine(boardRules);
            ShipsGame ships = new ShipsGame(generator, engine);
            ships.PrintHelp();
            ships.Simulation();
        }
    }
}

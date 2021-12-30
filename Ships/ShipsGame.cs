using System;
using ShipsEngine;
using ScreenGenerator;

namespace Ships
{
    /// <summary>
    /// ShipsGame class is used to control game flow.
    /// </summary>
    public class ShipsGame
    {
        private IScreenGenerator generator;
        private IBoard player1Board;
        private IBoard player2Board;
        private IBoard currentPlayerBoard;

        public ShipsGame(IScreenGenerator generator, IEngine engine)
        {
            this.generator = generator;
            player1Board = engine.PlaceShips();
            player2Board = engine.PlaceShips();
        }
        
        /// <summary>
        /// Start simulation between two computers
        /// </summary>
        public void Simulation()
        {
            Random random = new Random();
            generator.Clear();
            currentPlayerBoard = player1Board;
            do
            {
                int randomPlace = random.Next(0, currentPlayerBoard.HitedPlaces.Count);
                int placeToHit = currentPlayerBoard.HitedPlaces[randomPlace];
                bool wasHitted = currentPlayerBoard.HitShip(placeToHit);
                if (!wasHitted)
                {
                    SwichPlayer();
                }
                generator.DrawShips(player1Board);
                generator.DrawShips(player2Board);
                Console.ReadLine();
            }
            while (!IsPlayerWinner(player1Board) && !IsPlayerWinner(player2Board));
        }

        /// <summary>
        /// Print help menu.
        /// </summary>
        public void PrintHelp() => generator.PrintHelp();
        private void SwichPlayer() => currentPlayerBoard = currentPlayerBoard == player1Board ? player2Board : player1Board;
        private bool IsPlayerWinner(IBoard player) => player.ShipsLeft == 0;
    }
}

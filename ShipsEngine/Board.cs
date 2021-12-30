using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipsEngine
{
    /// <summary>
    /// Board class
    /// Contain board game array and ships
    /// </summary>
    public class Board : IBoard
    {
        BoardConstraints boardRules;
        public char[] GameBoard { get { return board; } }
        private char[] board;
        private int shipsLeft;
        public int ShipsLeft => shipsLeft;
        public int[] Ships => boardRules.Ships;
        private List<int> hitedPlaces = new List<int>();
        public List<int> HitedPlaces
        {
            get { return hitedPlaces; }
        }

       public Board(char[] board, BoardConstraints rules)
        {
            boardRules = rules;
            if (!BoardSizeFits(board.Length))
            {
                throw new ArgumentOutOfRangeException($"Border size should be between 0 and {boardRules.BoardSize}");
            }
            this.board = board;
            shipsLeft = Ships.Sum();
            hitedPlaces = Enumerable.Range(0, boardRules.BoardSize).ToList();
        }

        /// <summary>
        /// Clear the board and reset settings to default.
        /// </summary>
        public void ClearBoard()
        {
            Array.Fill(board, boardRules.EmptySpace);
            shipsLeft = Ships.Sum();
        }

        /// <summary>
        /// Check if ship exists in the selected place
        /// </summary>
        /// <param name="place">Place to shot</param>
        /// <returns>return true if ship was hitted</returns>
        public bool HitShip(int place)
        {
            if (place < 0 || place >= boardRules.BoardSize)
            {
                throw new IndexOutOfRangeException($"Place should be in range between 0 and {boardRules.BoardSize}");
            }
            hitedPlaces.RemoveAll(x => x == place);

            if (board[place] != boardRules.NormalShip)
            {
                board[place] = boardRules.HitedSpace;
                return false;
            }

            board[place] = boardRules.HitedShip;
            --shipsLeft;
            return true;
        }
        private bool BoardSizeFits(int size)
        {
            if (size > boardRules.BoardSize)
            {
                throw new IndexOutOfRangeException($"Board size could not be bigger than {boardRules.BoardSize}");
            }
            return true;
        }
    }
}

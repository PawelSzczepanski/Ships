using ShipsEngine;
using System;
using System.Linq;
using System.Text;

namespace ScreenGenerator
{
    /// <summary>
    /// Console implementation of IScreenGenerator
    /// </summary>
    public class ScreenGenerator : IScreenGenerator
    {
        private string frameString = "   ----------------------  ";
        private readonly BoardConstraints boardConstraints;

        public ScreenGenerator(BoardConstraints boardConstraints)
        {
            this.boardConstraints = boardConstraints;
        }

        /// <summary>
        /// Draw ship board in console.
        /// </summary>
        /// <param name="board"></param>
        public void DrawShips(IBoard board)
        {
            Console.WriteLine(DrawBoard(board));
        }

        /// <summary>
        /// Print help menu.
        /// </summary>
        public void PrintHelp()
        {
            Clear();
            Console.WriteLine("Welcome to ships console game!");
            Console.WriteLine(@"Game rules could be found here: https://en.wikipedia.org/wiki/Battleship_(game)");
        }

        /// <summary>
        /// Clear console.
        /// </summary>
        public void Clear() => Console.Clear();

        private string DrawBoard(IBoard board)
        {
            StringBuilder result = new StringBuilder();
            result.Append(CreateHeader());
            for (int i = 0; i < boardConstraints.RowSize; ++i)
            {
                result.Append(CreateRowNumber(i));
                for (int j = 0; j < boardConstraints.RowSize; ++j)
                {
                    int index = i * boardConstraints.RowSize + j;
                    char currentPositionChar = ReplaceEmptyCharWithSpace(board.GameBoard[index]);
                    result.Append(currentPositionChar);
                    result.Append(' ');
                }
                result.Append($"|{Environment.NewLine}");
            }
            result.AppendLine(frameString);
            return result.ToString();
        }

        private char ReplaceEmptyCharWithSpace(char charToChange)
        {
            if (charToChange == boardConstraints.EmptySpace)
            {
                return ' ';
            }
            return charToChange;
        }

        private string CreateHeader()
        {
            string headerString = "    A B C D E F G H I J  ";
            StringBuilder result = new StringBuilder();
            result.AppendLine(headerString);
            result.AppendLine(frameString);
            return result.ToString();
        }

        private string CreateRowNumber(int number)
        {
            const int digitNumbers = 2;
            string row_number = ($"{number + 1}");
            string paddedNumber = row_number.PadLeft(digitNumbers, ' ');
            return paddedNumber + "| ";
        }
    }
}

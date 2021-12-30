using Xunit;
using ShipsEngine;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Linq;
using System;

namespace EngineUnitTest
{
    public class EngineTest
    {
        int shipSize = 5;
        PrivateObject privateObject;
        BoardConstraints boardRules;
        Engine engine;

        public EngineTest()
        {
            boardRules = new BoardConstraints(rowSize: 10, ships: new[] { 5, 4, 4, 3, 3, 2 });
            engine = new Engine(boardRules);
            privateObject = new PrivateObject(engine);
        }

        /// <summary>
        /// Check if ship could be safely placed in board.
        /// We only check 3x5 area.
        /// Then we check if collision occur or not.
        /// In this scenario we expect that ship could not be safely placed.
        /// </summary>
        [Fact]
        public void ShipCouldNotBePlacedIfThereIsOtherObject()
        {
            int scope = 3; // we check only three adjacent places
            Board board = new Board(new char[boardRules.BoardSize], boardRules);
            int shipSize = 5;
            for (int i = 0; i < scope; ++i)
            {
                for (int j = 0; j < shipSize * boardRules.RowSize; j+= boardRules.RowSize)
                {
                    board.ClearBoard();
                    board.GameBoard[i + j] = 'x';
                    int place = j + 1;
                    bool result = engine.CouldBePlaced(board.GameBoard, shipSize, place, ShipOrientation.vertical);
                    Assert.IsFalse(result);
                }
            }
        }

        /// <summary>
        /// We check if ship could be safetly placed horizontally on the board.
        /// We assume that ship is 5 units long and is placed horizontally.
        /// For cell 0 - 4 ship could be placed safetly.
        /// For cells 4 - 9 ship could not be palced safetly.
        /// </summary>
        [Fact]
        public void HorizontalScope()
        {
            // We assume that max correct position is 5
            // For 5 cells ship it will take 5, 6, 7, 8, 9 cells
            // So with index starting from 0 it will be position 6
            const int skipPosition = 6;
            bool[] resultBoardRules = new bool[boardRules.RowSize];
            for (int i = 0; i < boardRules.RowSize; ++i)
            {
                for (int j = 0; j < boardRules.RowSize; ++j)
                {
                    int currentIndex = i * boardRules.RowSize + j;
                    resultBoardRules[j] = (bool)privateObject.Invoke("IsShipFittingToPlace", new object[] { shipSize, currentIndex, ShipOrientation.horizontal });
                }
                bool firstHalf = resultBoardRules.Take(skipPosition).All(x => x == true);
                bool secondHalf = resultBoardRules.Skip(skipPosition).All(x => x == false);
                Assert.IsTrue(firstHalf);
                Assert.IsTrue(secondHalf);
            }           
        }

        /// <summary>
        /// We check if ship could be safetly placed vertically.
        /// We assume that ship is 5 units long and it is located vertically.
        /// Ship should be placed safetly for all cells below 59 including 59.
        /// </summary>
        [Fact]
        public void VerticalScope()
        {
            // Max correct position for vertical orientation is 59
            // For 5 cells ship his position will be 59, 69, 79, 89, 99
            const int maxCorrectPosition = 59;
            bool[] resultBoardRules = new bool[boardRules.BoardSize];
            for (int i = 0; i < boardRules.BoardSize; ++i)
            {
                int currentIndex = i;
                resultBoardRules[i] = (bool)privateObject.Invoke("IsShipFittingToPlace", new object[] { shipSize, currentIndex, ShipOrientation.vertical });                
            }
            bool firstHalf = resultBoardRules.Take(maxCorrectPosition).All(x => x == true);
            bool secondHalf = resultBoardRules.Skip(maxCorrectPosition+1).All(x => x == false);
            Assert.IsTrue(firstHalf);
            Assert.IsTrue(secondHalf);

        }

        /// <summary>
        /// To place ship safetly we have to check if there are no object in sorround places.
        /// We should check position from -1 to +1 except two cases
        /// In example below first ship cell is located in A5 cell
        /// 
        ///   1 2 3 4 5 6 7 8 9 10
        ///   A    -1 0 1
        ///   B
        ///   C
        ///   
        /// First exception is when ship is located in the left corner, then we should check only 0 and +1 position
        /// Second exception is when ship is located in right corner, then we should check only -1 and 0 position
        /// Testing fuction return tupple where first value is loop start condition and second is loop stop condition
        /// </summary>
        [Fact]
        public void LeftToRightCheck()
        {
            const int leftBorderIndex = 0;
            const int rightBorderIndex = 9;
            foreach (ShipOrientation orientation in Enum.GetValues(typeof(ShipOrientation)))
            {
                for (int place = 0; place < boardRules.BoardSize; ++place)
                {
                    int correctStart;
                    int correctStop;
                    bool isShipHorizontal = orientation == ShipOrientation.horizontal;
                    bool fitts = (bool)privateObject.Invoke("IsShipFittingToPlace", new object[] { shipSize, place, orientation });
                    if (!fitts)
                    {
                        continue;
                    }
                    var start = (int)privateObject.Invoke("LoopStartIndex", new object[] { place, ShipOrientation.horizontal });
                    var stop = (int)privateObject.Invoke("LoopStopIndex", new object[] { place, shipSize, orientation, ShipOrientation.horizontal });
                    int normalizedPosition = place % boardRules.RowSize;
                    int shipWith = orientation == ShipOrientation.horizontal ? shipSize - 1 : 0;
                    bool leftSide = normalizedPosition == leftBorderIndex;
                    bool rightSide = normalizedPosition + shipWith == rightBorderIndex;
                    if (leftSide)
                    {
                        correctStart = 0;
                        correctStop = isShipHorizontal ? shipSize + 1 : 1;
                    }
                    else if (rightSide)
                    {
                        correctStart = -1;
                        correctStop = isShipHorizontal ? shipSize : 0;
                    }
                    else
                    {
                        correctStart = -1;
                        correctStop = isShipHorizontal ? shipSize + 1 : 1;
                    }
                    Assert.IsTrue(start == correctStart);
                    Assert.IsTrue(stop == correctStop);
                }
            }
        }

        /// <summary>
        /// To place ship safetly we have to check if there are no object in sorround places.
        /// We should check position from -rowSize to +rowSize except two cases
        /// In example below first ship cell is located in B5 cell (where r is rowSize)
        /// 
        ///   1 2 3 4 5 6 7 8 9 10
        ///   A    
        ///   B    -r 0 r
        ///   C
        ///   
        /// First exception is when ship is located in the upper side, then we should check only 0 and +rowSize position
        /// Second exception is when ship is located in bottom side, then we should check only -rowSize and 0 position
        /// Testing fuction return tupple where first value is loop start condition and second is loop stop condition
        /// </summary>
        [Fact]
        public void UpToBottomCheck()
        {
            const int bottomBorder = 90;
            foreach (ShipOrientation orientation in Enum.GetValues(typeof(ShipOrientation)))
            {
                for (int place = 0; place < boardRules.BoardSize; ++place)
                {
                    int correctStart;
                    int correctStop;
                    bool isShipVertical = orientation == ShipOrientation.vertical;
                    bool fitts = (bool)privateObject.Invoke("IsShipFittingToPlace", new object[] { shipSize, place, orientation });
                    if (!fitts)
                    {
                        continue;
                    }
                    var start = (int)privateObject.Invoke("LoopStartIndex", new object[] { place, ShipOrientation.vertical });
                    var stop = (int)privateObject.Invoke("LoopStopIndex", new object[] { place, shipSize, orientation, ShipOrientation.vertical });
                    int shipScope = orientation == ShipOrientation.vertical ? (shipSize - 1) * boardRules.RowSize : shipSize -1;

                    bool upperBorder = place < boardRules.RowSize;
                    bool downBorder = place + shipScope  >= bottomBorder;
                    if (upperBorder)
                    {
                        correctStart = 0;
                        correctStop = isShipVertical ? shipSize + 1 : 1;
                    }
                    else if (downBorder)
                    {
                        correctStart = -1;
                        correctStop = isShipVertical ? shipSize : 0;
                    }
                    else
                    {
                        correctStart = -1;
                        correctStop = isShipVertical ? shipSize + 1 : 1;
                    }
                    Assert.IsTrue(start == correctStart);
                    Assert.IsTrue(stop == correctStop);
                }
            }
        }
    }
}

using System;

namespace ShipsEngine
{
    public class Engine : IEngine
    {
        private readonly BoardConstraints boardConstraints;
        public Engine(BoardConstraints boardConstraints)
        {
            this.boardConstraints = boardConstraints;
        }

        /// <summary>
        /// Based on the data inside board constraint:
        /// Create new board.
        /// Place all ships on the board in random place.
        /// </summary>
        /// <returns>Return Board object filled with ships</returns>
        public IBoard PlaceShips()
        {
            const int numberOfOrientations = 2;
            Random random = new Random();
            char[] board = new char[boardConstraints.BoardSize];
            foreach (var ship in boardConstraints.Ships)
            {
                bool shipPlaced;
                do
                {
                    int xPos = random.Next(0, boardConstraints.RowSize);
                    int yPos = random.Next(0, boardConstraints.RowSize);
                    int shipPosition = xPos + yPos * boardConstraints.RowSize;
                    ShipOrientation orientation = (ShipOrientation)random.Next(0, numberOfOrientations);
                    shipPlaced = PlaceShip(board, ship, shipPosition, orientation);
                }
                while (!shipPlaced);
            }
            return new Board(board, boardConstraints);
        }


        
        /// <summary>
        /// Place single ship on the board.
        /// </summary>
        /// <param name="gameBoard">board array</param>
        /// <param name="shipSize">size of the ship</param>
        /// <param name="desiredShipLocation">place where to put ship</param>
        /// <param name="shipOrientation">Orientation of the ship</param>
        /// <returns>return True if ship could be placed on desiredShipLocation</returns>
        public bool PlaceShip(char[] gameBoard, int shipSize, int desiredShipLocation, ShipOrientation shipOrientation)
        {
            int cellShift = ComputeCellDistance(shipOrientation);
            if (CouldBePlaced(gameBoard, shipSize, desiredShipLocation, shipOrientation))
            {
                for (int i = 0; i < shipSize; ++i)
                {
                    gameBoard[desiredShipLocation + i * cellShift] = boardConstraints.NormalShip;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if ship could be placed in the desiredShipLocation
        /// </summary>
        /// <param name="gameBoard">board array</param>
        /// <param name="shipSize">size of the ship</param>
        /// <param name="desiredShipLocation">place where to put ship</param>
        /// <param name="shipOrientation">Orientation of the ship</param>
        /// <returns>return True if ship could be placed on desiredShipLocation</returns>
        public bool CouldBePlaced(char[] gameBoard, int shipSize, int desiredShipLocation, ShipOrientation shipOrientation)
        {
            if (!IsInRange(desiredShipLocation) || !IsShipFittingToPlace(shipSize, desiredShipLocation, shipOrientation))
            {
                return false;
            }
            int fromLeft = LoopStartIndex(desiredShipLocation, ShipOrientation.horizontal);
            int fromUp = LoopStartIndex(desiredShipLocation, ShipOrientation.vertical);
            int toLeft = LoopStopIndex(desiredShipLocation, shipSize, shipOrientation, ShipOrientation.horizontal);
            int toDown = LoopStopIndex(desiredShipLocation, shipSize, shipOrientation, ShipOrientation.vertical);

            for (int i = fromLeft; i <= toLeft; ++i)
            {
                for (int j = fromUp; j <= toDown; ++j)
                {
                    int placeToCheck = i + j * boardConstraints.RowSize + desiredShipLocation;
                    if (!IsInRange(placeToCheck) || gameBoard[placeToCheck] != boardConstraints.EmptySpace)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int ComputeCellDistance(ShipOrientation orientation) => orientation == ShipOrientation.horizontal ? 1 : boardConstraints.RowSize;
        private bool IsInRange(int placeToCheck) => placeToCheck < boardConstraints.BoardSize && placeToCheck >= 0;

        /// <summary>
        /// Compute normalized position
        /// For horizontal ship orientation it is position in a row (position of ship mod row size)
        /// For vertical ship orientation it is position in a board (should not change)
        /// </summary>
        /// <param name="shipPosition">position of a ship</param>
        /// <param name="shipOrientation">orientation of a ship</param>
        /// <returns>Position relative in row/board</returns>
        private int ComputeNormalizedPosition(int shipPosition, ShipOrientation shipOrientation) =>
            shipPosition % (ComputeCellDistance(shipOrientation) * boardConstraints.RowSize);

        /// <summary>
        /// Normalized size is a index of last ship cell
        /// It is better to create function instead of explaing it everywhere
        /// </summary>
        /// <param name="shipSize">Normalized size of the ship</param>
        /// <returns></returns>
        private int NormalizedShipSize(int shipSize) => --shipSize;
        private int ComputeBonduary(ShipOrientation orientation) => orientation == ShipOrientation.horizontal ? boardConstraints.RowSize : boardConstraints.BoardSize;
        private bool IsShipFittingToPlace(int shipSize, int place, ShipOrientation orientation)
        {
            int cellDistance = ComputeCellDistance(orientation);
            int boardBonduary = ComputeBonduary(orientation);
            int normalizedSize = NormalizedShipSize(shipSize);
            int normalizedPlace = ComputeNormalizedPosition(place, orientation);

            if ((normalizedSize * cellDistance + normalizedPlace) > (boardBonduary - 1))
            {
                return false;
            }
            return true;
        }
        private int LoopStartIndex(int position, ShipOrientation usage)
        {
            int cellDistance = ComputeCellDistance(usage);
            int normalizedPosition = position % (cellDistance * boardConstraints.RowSize);
            return (normalizedPosition - cellDistance) < 0 ? 0 : -1;
        }

        private int LoopStopIndex(int position, int shipSize, ShipOrientation shipOrientation, ShipOrientation usage)
        {
            int normalizedPosition = ComputeNormalizedPosition(position, usage);
            if (usage == shipOrientation)
            {
                if (normalizedPosition + shipSize * ComputeCellDistance(usage) < ComputeBonduary(usage))
                {
                    return shipSize + 1;
                }
                return shipSize;
            }
            else 
            {   
                if (normalizedPosition + ComputeCellDistance(usage) < ComputeBonduary(usage))
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

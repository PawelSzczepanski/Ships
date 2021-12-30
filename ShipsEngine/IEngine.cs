namespace ShipsEngine
{
    public interface IEngine
    {
        IBoard PlaceShips();
        bool PlaceShip(char[] board, int shipSize, int shipInitialPosition, ShipOrientation shipOrientation);
        bool CouldBePlaced(char[] board, int shipSize, int place, ShipOrientation orientation);
    }
}
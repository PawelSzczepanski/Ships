namespace ShipsEngine
{
    /// <summary>
    /// Class contains all constraints used in Ships game.
    /// </summary>
    public class BoardConstraints
    {
        public readonly char EmptySpace = '\0';
        public readonly char NormalShip = 'o';
        public readonly char HitedShip = 'x';
        public readonly char HitedSpace = '$';

        public readonly int RowSize;
        public readonly int BoardSize;
        public readonly int[] Ships;

        public BoardConstraints(int rowSize, int[] ships)
        {
            RowSize = rowSize;
            Ships = ships;
            BoardSize = RowSize * RowSize;
        }
    }
}

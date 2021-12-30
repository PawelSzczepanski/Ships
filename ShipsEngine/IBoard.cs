using System.Collections.Generic;

namespace ShipsEngine
{
    public interface IBoard
    {
        void ClearBoard();
        public bool HitShip(int place);
        int ShipsLeft { get; }
        char[] GameBoard { get; }
        List<int> HitedPlaces { get; }
    }
}

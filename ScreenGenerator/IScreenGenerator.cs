using ShipsEngine;

namespace ScreenGenerator
{
    /// <summary>
    /// Basic screen operations contract.
    /// </summary>
    public interface IScreenGenerator
    { 
        void DrawShips(IBoard board);
        void PrintHelp();
        void Clear();
    }
}
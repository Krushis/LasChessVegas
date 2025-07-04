

using System.Xml;

namespace VegasBackend.Models.Pieces
{
    public abstract class Piece
    {
        public bool IsWhite { get; set; } // ko reikia tai color, notation, 
        public abstract string Notation { get; }
        public abstract List<string> GetLegalMoves(ChessBoard board);
    }
}

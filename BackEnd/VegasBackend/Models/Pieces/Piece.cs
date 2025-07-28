

using System.Xml;

namespace VegasBackend.Models.Pieces
{
    public abstract class Piece
    {
        public bool IsWhite { get; set; }
        public abstract string Notation { get; }
        public abstract List<string> GetLegalMoves(string[][] board);
    }
}

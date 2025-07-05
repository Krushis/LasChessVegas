using System.Net.NetworkInformation;
using VegasBackend.Models.Pieces;

namespace VegasBackend.Models
{
    public class PieceHelper
    {
        public static Piece GetPieceFromCode(string code, int col, int row)
        {
            if (string.IsNullOrWhiteSpace(code) || code == "-")
                return null;

            bool isWhite = code[0] == 'w';
            char pieceType = char.ToUpper(code[1]);

            return pieceType switch
            {
                'P' => new Pawn(col, row) { IsWhite = isWhite },
                'R' => new Rook(col, row) { IsWhite = isWhite },
                'N' => new Knight(col, row) { IsWhite = isWhite },
                'B' => new Bishop(col, row) { IsWhite = isWhite },
                'Q' => new Queen(col, row) { IsWhite = isWhite },
                'K' => new King(col, row) { IsWhite = isWhite },
                _ => null
            };
        }

    }
}

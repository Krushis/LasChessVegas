namespace VegasBackend.ServerLogic.ChessAI
{
    public static class Hashing
    {
        private static ulong[,,] _pieceKeys; // [row][col][pieceType]
        private static ulong _blackToMove;
        private static bool _initialized = false;

        private static readonly Dictionary<string, int> PieceIndices = new()
        {
            { "wp", 0 }, { "wN", 1 }, { "wB", 2 }, { "wR", 3 }, { "wQ", 4 }, { "wK", 5 },
            { "bp", 6 }, { "bN", 7 }, { "bB", 8 }, { "bR", 9 }, { "bQ", 10 }, { "bK", 11 }
        };

        public static void Initialize()
        {
            if (_initialized) return;

            var rng = new Random(123456789);
            _pieceKeys = new ulong[8, 8, 12];

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    for (int piece = 0; piece < 12; piece++)
                    {
                        _pieceKeys[row, col, piece] = NextULong(rng);
                    }
                }
            }

            _blackToMove = NextULong(rng);
            _initialized = true;
        }

        private static ulong NextULong(Random rng)
        {
            byte[] buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong ComputeHash(string[][] board, bool isWhiteTurn)
        {
            if (!_initialized) Initialize();

            ulong hash = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string piece = board[row][col];
                    if (piece != "-" && PieceIndices.TryGetValue(piece, out int pieceIndex))
                    {
                        hash ^= _pieceKeys[row, col, pieceIndex];
                    }
                }
            }

            if (!isWhiteTurn)
                hash ^= _blackToMove;

            return hash;
        }

        public static ulong UpdateHash(ulong currentHash, int fromRow, int fromCol, int toRow, int toCol,
                                       string piece, string capturedPiece, bool flipTurn = true)
        {
            if (!_initialized) Initialize();

            ulong hash = currentHash;

            if (PieceIndices.TryGetValue(piece, out int pieceIndex))
            {
                hash ^= _pieceKeys[fromRow, fromCol, pieceIndex];
                hash ^= _pieceKeys[toRow, toCol, pieceIndex];
            }

            if (capturedPiece != "-" && PieceIndices.TryGetValue(capturedPiece, out int capIndex))
            {
                hash ^= _pieceKeys[toRow, toCol, capIndex];
            }

            if (flipTurn)
                hash ^= _blackToMove;

            return hash;
        }
    }
}

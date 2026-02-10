namespace VegasBackend.ServerLogic.ChessAI
{
    public static class Evaluation
    {
        private static readonly Dictionary<char, int> PieceValue = new()
        {
            { 'p', 100 },
            { 'n', 320 },
            { 'b', 330 },
            { 'r', 500 },
            { 'q', 900 },
            { 'k', 20000 }
        };

        private static readonly int[] PawnPST = {
            0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
             5,  5, 10, 25, 25, 10,  5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5, -5,-10,  0,  0,-10, -5,  5,
             5, 10, 10,-20,-20, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };

        private static readonly int[] KnightPST = {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        };

        private static readonly int[] WhitePawnValues = new int[64];
        private static readonly int[] BlackPawnValues = new int[64];
        private static readonly int[] WhiteKnightValues = new int[64];
        private static readonly int[] BlackKnightValues = new int[64];

        private static readonly Dictionary<char, int> BasePieceValues = new()
        {
            { 'b', 330 }, { 'r', 500 }, { 'q', 900 }, { 'k', 20000 }
        };

        static Evaluation()
        {
            for (int i = 0; i < 64; i++)
            {
                // Pawn Setup
                WhitePawnValues[i] = 100 + PawnPST[i];
                BlackPawnValues[i] = 100 + PawnPST[((7 - (i / 8)) * 8 + (i % 8))];

                // Knight Setup
                WhiteKnightValues[i] = 320 + KnightPST[i];
                BlackKnightValues[i] = 320 + KnightPST[((7 - (i / 8)) * 8 + (i % 8))];
            }
        }

        public static int EvaluateBoard(string[][] board)
        {
            int score = 0;
            for (int r = 0; r < 8; r++)
            {
                var row = board[r];
                int rowOffset = r * 8;

                for (int c = 0; c < 8; c++)
                {
                    string piece = row[c];
                    if (piece == "-") continue;

                    int idx = rowOffset + c;
                    bool isWhite = piece[0] == 'w';
                    char type = piece[1];

                    int val;
                    if (type == 'p' || type == 'P')
                        val = isWhite ? WhitePawnValues[idx] : BlackPawnValues[idx];
                    else if (type == 'n' || type == 'N')
                        val = isWhite ? WhiteKnightValues[idx] : BlackKnightValues[idx];
                    else
                    {
                        char lowerType = char.ToLower(type);
                        val = BasePieceValues[lowerType];
                    }

                    score += isWhite ? val : -val;
                }
            }
            return score;
        }

        public static int GetPieceValue(char piece)
        {
            if (PieceValue.TryGetValue(piece, out int value))
                return value;
            return 0;
        }
    }

}

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

        /// <summary>
        /// Evaluates the board based on piece strength
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int EvaluateBoard(string[][] board)
        {
            int score = 0;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var piece = board[r][c];
                    if (piece == "-") continue;

                    bool isWhite = piece[0] == 'w';
                    char type = char.ToLower(piece[1]);

                    int value = PieceValue[type];
                    score += isWhite ? value : -value;
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

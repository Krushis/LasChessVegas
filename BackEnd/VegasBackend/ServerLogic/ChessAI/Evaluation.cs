namespace VegasBackend.ServerLogic.ChessAI
{
    public static class Evaluation
    {
        private static readonly Dictionary<char, int> PieceValue = new()
        {
            { 'p', 1 },
            { 'n', 3 },
            { 'b', 3 },
            { 'r', 5 },
            { 'q', 9 },
            { 'k', 100 }
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
    }

}

using VegasBackend.DTO;
using VegasBackend.Models;
using VegasBackend.ServerLogic.ChessAI;

namespace VegasBackend.ServerLogic
{
    /// <summary>
    /// Utils for AI service for game state
    /// </summary>
    public static class GameStateUtils
    {
        public static GameState ApplyMove(GameState state, DTOLegalMove move)
        {
            var newBoard = state.Board
                .Select(row => row.ToArray())
                .ToArray();

            var newMoves = new List<string>(state.MadeMoves);

            AnnotationHelper.SimulateMove(ref newBoard, move.Move);

            newMoves.Add(move.Move);

            var newState = new GameState
            {
                Board = newBoard,
                MadeMoves = new List<string>(state.MadeMoves) { move.Move },
                MoveCount = state.MoveCount + 1,
                Hash = Hashing.ComputeHash(newBoard, (state.MoveCount + 1) % 2 == 0)
            };

            return newState;
        }
    }
}

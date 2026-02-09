using VegasBackend.DTO;
using VegasBackend.Models;

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

            return new GameState
            {
                Board = newBoard,
                MadeMoves = newMoves,
                MoveCount = state.MoveCount + 1
            };
        }
    }
}

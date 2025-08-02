using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using VegasBackend.DTO;
using VegasBackend.Models;
using VegasBackend.Models.Pieces;
using VegasBackend.ServerLogic;

namespace VegasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This method by logic should only be run at the start of the game
        /// </summary>
        /// <returns>Request with the data of the board</returns>
        [HttpGet("/initializeAndGetBoard")]
        public IActionResult GetBoard() // TODO create a db that would save the board info based on gameID, for now lets focus on 
            // game logic i think
        {
            try
            {
                var gameID = Guid.NewGuid().ToString();

                ChessBoard board = new ChessBoard();
                board.InitializeBoard();

                var gameState = new GameState
                {
                    Board = board.board
                };

                GameStore.Games[gameID] = gameState;

                if (board == null)
                {
                    NotFound();
                }

                return Ok(new
                {
                    success = true,
                    gameId = gameID,
                    board = gameState.Board
                });
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in GetBoard - " + ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error trying to get the board" });
            }
        }

        [HttpPost("/GetLegalMoves")]
        public IActionResult GetLegalMoves([FromBody] GameIdDTO request) // stateless
        {
            try
            {
                if (!GameStore.Games.TryGetValue(request.GameId, out var gameState))
                    return NotFound();

                string colorLetter = gameState.MoveCount % 2 == 0 ? "w" : "b";

                var legalMoves = new List<LegalMoveDTO>();

                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        var pieceCode = gameState.Board[row][col];
                        if (pieceCode != "-" && pieceCode.StartsWith(colorLetter))
                        {
                            var piece = PieceHelper.GetPieceFromCode(pieceCode, col, row);
                            var beforeCheckMoves = piece.GetLegalMoves(gameState.Board, gameState.LastMove);

                            foreach (var moveDTO in beforeCheckMoves)
                            {
                                var cloneBoard = ChessBoard.CloneBoard(gameState.Board);
                                AnnotationHelper.SimulateMove(ref cloneBoard, moveDTO.Move);

                                if (!AnnotationHelper.IsKingInCheck(cloneBoard, colorLetter == "w", gameState.LastMove))
                                {
                                    legalMoves.Add(moveDTO);
                                }
                            }
                        }
                    }
                }

                return Ok(legalMoves);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetLegalMoves - " + ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error trying to get legal moves" });
            }
        }

        [HttpPost("/MakeMove")]
        public IActionResult MakeMove([FromBody] MoveDTO moveObject)
        {
            try
            {
                if (moveObject == null || string.IsNullOrEmpty(moveObject.GameId))
                    return BadRequest(new { success = false, message = "Game ID missing" });

                if (!GameStore.Games.TryGetValue(moveObject.GameId, out var gameState))
                    return NotFound(new { success = false, message = "Game not found" });

                var fromPos = AnnotationHelper.AlgebraicToIndex(moveObject.From);
                var toPos = AnnotationHelper.AlgebraicToIndex(moveObject.To);
                if (fromPos == null || toPos == null)
                    return BadRequest(new { success = false, message = "Invalid positions" });

                string pieceCode = gameState.Board[fromPos.Value.Row][fromPos.Value.Col];
                if (pieceCode == "-")
                    return BadRequest(new { success = false, message = "No piece at source" });

                var piece = PieceHelper.GetPieceFromCode(pieceCode, fromPos.Value.Col, fromPos.Value.Row);
                var legalMoves = piece.GetLegalMoves(gameState.Board, gameState.LastMove);

                string moveString = moveObject.From + moveObject.To;
                if (!legalMoves.Any(m => m.Move == moveString))
                    return BadRequest(new { success = false, message = "Illegal move" });

                // Apply move
                gameState.Board[toPos.Value.Row][toPos.Value.Col] = pieceCode;
                gameState.Board[fromPos.Value.Row][fromPos.Value.Col] = "-";

                // Promotion logic
                if (moveObject.PromotionPiece != null && moveObject.PromotionPiece != "none")
                {
                    gameState.Board[toPos.Value.Row][toPos.Value.Col] = moveObject.PromotionPiece;
                }

                // Update game state
                gameState.LastMove = moveString;
                gameState.MoveCount++;

                return Ok(new
                {
                    success = true,
                    board = gameState.Board
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error executing move");
                return StatusCode(500, new { success = false, message = "Internal server error trying to make move" });
            }
        }

    }

}


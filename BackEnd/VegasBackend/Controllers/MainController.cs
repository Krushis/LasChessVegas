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
                    Board = board.board,
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
                            var beforeCheckMoves = piece.GetLegalMoves(gameState.Board, gameState.MadeMoves, false);

                            foreach (var moveDTO in beforeCheckMoves)
                            {
                                var cloneBoard = ChessBoard.CloneBoard(gameState.Board);
                                AnnotationHelper.SimulateMove(ref cloneBoard, moveDTO.Move);

                                if (!AnnotationHelper.IsKingInCheck(cloneBoard, colorLetter == "w", gameState.MadeMoves))
                                {
                                    legalMoves.Add(moveDTO);
                                }
                            }
                        }
                    }
                }
                _logger.LogInformation("Got legal moves, count - " +  legalMoves.Count);
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
                var legalMoves = piece.GetLegalMoves(gameState.Board, gameState.MadeMoves, false);

                string moveString = moveObject.From + moveObject.To;
                if (!legalMoves.Any(m => m.Move == moveString))
                    return BadRequest(new { success = false, message = "Illegal move" });

                // Check if the move is a castling move
                bool isKing = pieceCode.EndsWith("K");
                bool isCastlingMove = isKing && Math.Abs(toPos.Value.Col - fromPos.Value.Col) == 2;

                if (isCastlingMove)
                {
                    int rookFromCol, rookToCol;
                    int row = fromPos.Value.Row;

                    if (toPos.Value.Col > fromPos.Value.Col)
                    {
                        // Kingside castling
                        rookFromCol = 7;
                        rookToCol = 5;
                    }
                    else
                    {
                        // Queenside castling
                        rookFromCol = 0;
                        rookToCol = 3;
                    }

                    string rookPiece = gameState.Board[row][rookFromCol];
                    gameState.Board[row][rookToCol] = rookPiece;
                    gameState.Board[row][rookFromCol] = "-";
                }


                // Apply move
                gameState.Board[toPos.Value.Row][toPos.Value.Col] = pieceCode;
                gameState.Board[fromPos.Value.Row][fromPos.Value.Col] = "-";

                bool isWhite = pieceCode.StartsWith("w");
                bool isPawn = pieceCode.EndsWith("p");
                bool reachedLastRank = (isWhite && toPos.Value.Row == 0) || (!isWhite && toPos.Value.Row == 7);

                //_logger.LogInformation("111Made move - " + moveString);
                //_logger.LogInformation("Move count - " + gameState.MoveCount.ToString());

                // would be smart to figure out a way to block double moves when promotion window is up
                // that doesnt just include blocker window, but also provides backend validation, since right now you can just delete it

                // also met bug with queen when we promote it, for some reason it cant go back??
                if (moveObject.PromotionPiece != null && moveObject.PromotionPiece != "none")
                {
                    if (!isPawn || !reachedLastRank)
                    {
                        return BadRequest(new { success = false, message = "Invalid promotion: not a pawn at last rank" });
                    }

                    string promo = moveObject.PromotionPiece;

                    // Validate that it's a valid piece and matches the player's color
                    string[] allowedPromotions = { "Q", "R", "B", "N" };
                    bool valid = allowedPromotions.Any(p => promo == (isWhite ? "w" : "b") + p);

                    if (!valid)
                    {
                        return BadRequest(new { success = false, message = "Invalid promotion piece" });
                    }

                    // Apply the promotion
                    gameState.Board[toPos.Value.Row][toPos.Value.Col] = promo;
                }
                else if (isPawn && reachedLastRank)
                {
                    // Player did not submit a promotion piece, but one was required
                    return BadRequest(new { success = false, message = "Missing promotion piece for pawn" });
                }

                // Cant I just use the MadeMoves variable to check for castling eligiblity?
                _logger.LogInformation("Made move - " + pieceCode + moveString);

                // Update game state
                gameState.MadeMoves.Add(moveString);
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


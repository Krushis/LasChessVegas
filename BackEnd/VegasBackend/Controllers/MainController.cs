using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using VegasBackend.DTO;
using VegasBackend.Models;
using VegasBackend.Models.Pieces;
using VegasBackend.ServerLogic;
using VegasBackend.DbContex;
using System.Text.Json;

namespace VegasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly AppDbContex _dbContext;

        public MainController(ILogger<MainController> logger, AppDbContex context)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpPost("/CreateAndGetBoard")]
        public async Task<IActionResult> CreateAndGetBoard([FromBody] CreateGameDTO dto)
        {
            _logger.LogInformation("Got id - " + dto.Player1Id + dto.Player2Id);

            ChessBoard chess = new ChessBoard();
            chess.InitializeBoard();

            var newGame = new GameDb
            {
                Id = Guid.NewGuid(),
                BoardJson = JsonSerializer.Serialize(chess.board),
                MoveCount = 0,
                MadeMovesJson = JsonSerializer.Serialize(new List<string>()),
                Player1Id = dto.Player1Id,
                Player2Id = dto.Player2Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Games.Add(newGame);
            await _dbContext.SaveChangesAsync();

            var board = chess.board;

            return Ok(new
            {
                success = true,
                gameId = newGame.Id,
                board,
                madeMoves = new List<string>(),
                moveCount = 0,
                player1Id = newGame.Player1Id,
                player2Id = newGame.Player2Id
            });
        }

        [HttpPost("/CheckEndGame")]
        public async Task<IActionResult> CheckEndGame([FromBody] GameIdDTO request)
        {
            var game = await _dbContext.Games.FindAsync(request.GameId);
            if (game == null)
                return NotFound(new { success = false, message = "Game not found" });

            var board = JsonSerializer.Deserialize<string[][]>(game.BoardJson);
            var madeMoves = JsonSerializer.Deserialize<List<string>>(game.MadeMovesJson);

            _logger.LogInformation(board[0][0]);

            var gameState = new GameState // technically can go without this aswell
            {
                Board = board,
                MadeMoves = madeMoves,
                MoveCount = game.MoveCount
            };

            var result = EndGameChecking.CheckEndGame(gameState);

            return Ok(new { success = true, result });
        }

        [HttpPost("/GetLegalMoves")]
        public async Task<IActionResult> GetLegalMoves([FromBody] GameIdDTO request)
        {
            try
            {
                var game = await _dbContext.Games.FindAsync(request.GameId);
                if (game == null)
                    return NotFound(new { success = false, message = "Game not found" });

                var board = JsonSerializer.Deserialize<string[][]>(game.BoardJson);
                var madeMoves = JsonSerializer.Deserialize<List<string>>(game.MadeMovesJson);

                var gameState = new GameState // technically can go without this aswell
                {
                    Board = board,
                    MadeMoves = madeMoves,
                    MoveCount = game.MoveCount
                };

                bool isWhiteTurn = gameState.MoveCount % 2 == 0;
                var legalMoves = LegalMoveGenerator.GetAllLegalMoves(
                    gameState.Board,
                    gameState.MadeMoves,
                    isWhiteTurn
                );

                _logger.LogInformation("Got legal moves, count - " + legalMoves.Count);
                return Ok(legalMoves);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetLegalMoves - " + ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error trying to get legal moves" });
            }
        }

        [HttpPost("/MakeMove")]
        public async Task<IActionResult> MakeMove([FromBody] MoveDTO moveObject)
        {
            try
            {
                if (moveObject == null)
                    return BadRequest(new { success = false, message = "Game ID missing" });

                var game = await _dbContext.Games.FindAsync(moveObject.GameId);
                if (game == null)
                    return NotFound(new { success = false, message = "Game not found" });

                var board = JsonSerializer.Deserialize<string[][]>(game.BoardJson);
                var madeMoves = JsonSerializer.Deserialize<List<string>>(game.MadeMovesJson);

                var gameState = new GameState // technically can go without this aswell
                {
                    Board = board,
                    MadeMoves = madeMoves,
                    MoveCount = game.MoveCount
                };

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

                gameState.MadeMoves.Add(moveString);

                // Record the move
                game.BoardJson = JsonSerializer.Serialize(gameState.Board);
                game.MadeMovesJson = JsonSerializer.Serialize(gameState.MadeMoves);
                game.MoveCount = gameState.MoveCount; // Still the old count

                _dbContext.Games.Update(game);
                await _dbContext.SaveChangesAsync();

                // NOW increment move count for the next turn
                gameState.MoveCount++;

                // Update the DB again with the new move count
                game.MoveCount = gameState.MoveCount;
                _dbContext.Games.Update(game);
                await _dbContext.SaveChangesAsync();

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


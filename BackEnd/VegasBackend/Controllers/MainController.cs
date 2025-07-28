using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VegasBackend.DTO;
using VegasBackend.Models;

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
        public IActionResult GetBoard()
        {
            ChessBoard board = new ChessBoard();
            board.InitializeBoard();

            if(board == null)
            {
                NotFound();
            }

            return Ok(board);
        }

        [HttpPost("/GetLegalMoves")]
        public IActionResult GetLegalMoves([FromBody] LegalMovesDTO request) // stateless
        {
            try
            {
                List<string> legalMoves = new List<string>();

                _logger.LogInformation(request.MoveCount.ToString());
                string colorLetter = request.MoveCount % 2 == 0 ? "w" : "b";

                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if (request.Board[row][col] != "-" && request.Board[row][col].Substring(0, 1) == colorLetter)
                        {
                            var piece = PieceHelper.GetPieceFromCode(request.Board[row][col], col, row);
                            legalMoves.AddRange(piece.GetLegalMoves(request.Board));
                        }
                    }
                }

                _logger.LogInformation("Got all legal moves");

                return Ok(legalMoves);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in GetLegalMoves - " + ex.Message);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("/MakeMove")]
        public IActionResult MakeMove([FromBody] MoveDTO moveObject)
        {
            try
            {
                if (moveObject == null || string.IsNullOrEmpty(moveObject.From) ||
                string.IsNullOrEmpty(moveObject.To) || moveObject.Board == null)
                {
                    return BadRequest(new { success = false, message = "Invalid move data" });
                }

                var fromPosition = AnnotationHelper.AlgebraicToIndex(moveObject.From);
                var toPosition = AnnotationHelper.AlgebraicToIndex(moveObject.To);

                if (fromPosition == null || toPosition == null)
                {
                    return BadRequest(new { success = false, message = "Invalid algebraic notation" });
                }

                string pieceCode = moveObject.Board[fromPosition.Value.Row][fromPosition.Value.Col];
                if (pieceCode == "-")
                {
                    return BadRequest(new { success = false, message = "No piece at source position" });
                }

                var piece = PieceHelper.GetPieceFromCode(pieceCode, fromPosition.Value.Col, fromPosition.Value.Row);
                var legalMoves = piece.GetLegalMoves(moveObject.Board);
                string moveString = moveObject.From + moveObject.To;

                if (!legalMoves.Contains(moveString))
                {
                    return BadRequest(new { success = false, message = "Illegal move" });
                }

                moveObject.Board[toPosition.Value.Row][toPosition.Value.Col] = pieceCode;
                moveObject.Board[fromPosition.Value.Row][fromPosition.Value.Col] = "-";

                _logger.LogInformation("Made Move - " + moveString);

                return Ok(new
                {
                    success = true,
                    board = moveObject.Board,
                    message = "Move executed successfully"
                });

            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error executing move");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }




}


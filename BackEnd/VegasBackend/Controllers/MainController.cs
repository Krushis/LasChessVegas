using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetLegalMoves([FromBody] ChessBoard boardObject)
        {
            ChessBoard board = new ChessBoard();
            board.InitializeBoard();
            List<string> legalMoves = new List<string>();

            for(int row = 0; row < 8; row++)
            {
                for(int col = 0;  col < 8; col++)
                {
                    if (board.board[row][col] != "-")
                    {
                        var piece = PieceHelper.GetPieceFromCode(board.board[row][col], col, row);
                        legalMoves.AddRange(piece.GetLegalMoves(board));
                    }
                }
            }

            legalMoves.Add("a2a4");

            return Ok(legalMoves);
        }
    }




}


import {initializeBoardUI} from "./Board.js";
import {updateLegalMoves, makeAIMove}  from "./ApiLogic.js";

let isAIGame = false;
let playerColor = 'white';

async function main(aiMode = false) {
    isAIGame = aiMode;
    await initializeBoardUI(aiMode);
    await updateLegalMoves();
}

document.getElementById("start-game-button").addEventListener("click", async () => {
    await main(false);
    const chessBoard = document.querySelector(".chessBoard");
    chessBoard.classList.add("active");
});

document.getElementById("start-ai-game-button").addEventListener("click", async () => {
    await main(true);
    const chessBoard = document.querySelector(".chessBoard");
    chessBoard.classList.add("active");
});

document.getElementById("new-game-button").addEventListener("click", async () => {
    document.getElementById("endgame-modal").classList.add("hidden");
    document.getElementById("overlay-blocker").style.display = "none";
    
    await initializeBoardUI(isAIGame);
    await updateLegalMoves();
});

document.getElementById("endgame-modal").addEventListener("click", function(e) {
    if (e.target === this) {
        this.classList.add("hidden");
    }
});

document.addEventListener('dragend', function(e) {
    if (e.target.classList.contains('chessPiece')) {
        e.target.style.opacity = '1';
        e.target.classList.remove('highlighted-cell');
        const fromCell = document.getElementById(e.target.dataset.algebraic);
        fromCell.classList.remove("highlighted-cell");
    }
});

export { isAIGame, playerColor };
import {initializeBoardUI} from "./Board.js";
import "./DragPieces.js";
import "./ApiLogic.js";
import "./MoveLogic.js";
import {updateLegalMoves}  from "./ApiLogic.js";

main();

async function main() {

    await initializeBoardUI();
    await updateLegalMoves();
}

document.getElementById("new-game-button").addEventListener("click", async () => {
    document.getElementById("endgame-modal").classList.add("hidden");
    document.getElementById("overlay-blocker").style.display = "none";
    
    await initializeBoardUI();
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



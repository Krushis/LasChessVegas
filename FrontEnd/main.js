import AnnotationHelper from "./AnnotationHelper.js";
import FetchWrapper from "./ApiWrapper.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");
const annotationHelper = new AnnotationHelper();

let board = null;
let LegalMoves = [];
let MadeMoves = [];
let moveCount = 0;

main();

async function main() {

    await initializeBoard();
    await updateLegalMoves();
}


async function initializeBoard() {
    try {
        const jsonData = await fetchAPI.get("initializeAndGetBoard");
        board = jsonData.board;

        // board is a 2d array

        const boardDiv = document.querySelector(".chessBoard");
        boardDiv.innerHTML = "";

        for(let row = 0; row < 8; row++) {

            const rowDiv = document.createElement("div");
            rowDiv.className = 'row';

            for(let col = 0; col < 8; col++) {

                const cell = document.createElement("div");
                cell.className = "cell";

                cell.id = annotationHelper.indexToAlgebraic(col, row);
                cell.dataset.row = row;
                cell.dataset.col = col;

                if ((row + col) % 2 == 0) {
                    cell.classList.add("white");
                }
                else {
                    cell.classList.add("black");
                }

                const pieceAnnotation = board[row][col];
                if (pieceAnnotation != "-") {
                    const pieceImage = document.createElement("img");
                    pieceImage.src = `./assets/${pieceAnnotation}.png`;
                    pieceImage.alt = `${pieceAnnotation}`;
                    pieceImage.className = "chessPiece";

                    pieceImage.draggable = true;
                    pieceImage.dataset.pieceValue = pieceAnnotation;
                    pieceImage.dataset.row = row;
                    pieceImage.dataset.col = col;
                    pieceImage.dataset.algebraic = annotationHelper.indexToAlgebraic(col, row);
                    
                    pieceImage.addEventListener('dragstart', handleDragStart);

                    cell.appendChild(pieceImage);
                }

                cell.addEventListener('dragover', handleDragOver);
                cell.addEventListener('drop', handleDrop);
                
                rowDiv.appendChild(cell);
            }

            boardDiv.appendChild(rowDiv);

        }

    }

    catch(error) {
        console.error("Error trying to initialize board - " + error);
    }
    
};


let draggedPiece = null;
let draggedFrom = null;

function handleDragStart(event) {
    draggedPiece = event.target;
    draggedFrom = {
        row: parseInt(event.target.dataset.row),
        col: parseInt(event.target.dataset.col),
        piece: event.target.dataset.pieceValue,
        algebraic: event.target.dataset.algebraic
    };

    event.target.style.opacity = '0.5';
    
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/html', event.target.outerHTML);

}

function handleDragOver(event) {
    event.preventDefault(); // allows drop - essential
    event.dataTransfer.dropEffect = 'move';
}

function handleDrop(event) {
    event.preventDefault();

    if (!draggedPiece || !draggedFrom) return;

    // closest so that we can avoid dragging onto image instead of cell
    const dropCell = event.target.classList.contains('cell') ? event.target : event.target.closest('.cell');
    if (!dropCell) return;

    const dropRow = parseInt(dropCell.dataset.row);
    const dropCol = parseInt(dropCell.dataset.col);
    const dropAlgebraic = dropCell.id;

    const move = {
        from: draggedFrom.algebraic,
        to: dropAlgebraic,
        fromRow: draggedFrom.row,
        fromCol: draggedFrom.col,
        toRow: dropRow,
        toCol: dropCol
    };

    const moveString = move.from + move.to;

    const isLegalMove = LegalMoves.includes(moveString);

    if (isLegalMove) {
        executeMove(move, dropCell);
    } else {
        draggedPiece.style.opacity = '1';
        console.log("Illegal move:", moveString);
    }

    draggedPiece = null;
    draggedFrom = null;
}

async function executeMove(move, targetCell) {
    try{
        const moveData = {
            from: move.from,
            to: move.to,
            board: board,
            moveCount: moveCount
        };

        const response = await fetchAPI.post("MakeMove", moveData);

        if (response.success) {
            const originalCell = document.getElementById(move.from);
            const pieceToMove = originalCell.querySelector('.chessPiece');
            
            if (pieceToMove) {
                const existingPiece = targetCell.querySelector('.chessPiece');
                if (existingPiece) {
                    existingPiece.remove();
                }

                pieceToMove.style.opacity = '1';
                pieceToMove.dataset.row = move.toRow;
                pieceToMove.dataset.col = move.toCol;
                pieceToMove.dataset.algebraic = move.to;
                
                targetCell.appendChild(pieceToMove);
                
                board[move.toRow][move.toCol] = board[move.fromRow][move.fromCol];
                board[move.fromRow][move.fromCol] = "-";

                MadeMoves.push(move);
                moveCount++;
                await updateLegalMoves();
            }
        } else {
            draggedPiece.style.opacity = '1';
            console.log("Move not allowed by the backend, move - " + move.from + move.to);
        }

    }
    catch(error) {
        console.log("Found error in executeMove - " + error);
        draggedPiece.style.opacity = '1';
    }
}

async function updateLegalMoves() {
    try {
        const lastMove = MadeMoves.length > 0 ? MadeMoves[MadeMoves.length - 1] : null;
        LegalMoves = await fetchAPI.post("GetLegalMoves", {
        board: board,
        lastMove: lastMove,
        moveCount: moveCount
    });
        console.log(LegalMoves);
        console.log()
    } catch (error) {
        console.error("Error updating legal moves: " + error);
    }
}

document.addEventListener('dragend', function(e) {
    if (e.target.classList.contains('chessPiece')) {
        e.target.style.opacity = '1';
    }
});


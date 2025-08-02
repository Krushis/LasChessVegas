import AnnotationHelper from "./AnnotationHelper.js";
import FetchWrapper from "./ApiWrapper.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");
const annotationHelper = new AnnotationHelper();

let board = null;
let LegalMoves = [];
let MadeMoves = [];
let moveCount = 0;
let gameId = null;


main();

async function main() {

    await initializeBoard();
    await updateLegalMoves();
}


async function initializeBoard() {
    try {
        const jsonData = await fetchAPI.get("initializeAndGetBoard");
        board = jsonData.board;
        gameId = jsonData.gameId;

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
    
    const legalMoveDTO = LegalMoves.find(legalMove => legalMove.move === moveString);
    const isLegalMove = legalMoveDTO !== undefined;

    if (isLegalMove) {
        executeMove(move, dropCell, legalMoveDTO);
    } else {
        draggedPiece.style.opacity = '1';
        console.log("Illegal move:", moveString);
    }

    draggedPiece = null;
    draggedFrom = null;
}

async function executeMove(move, targetCell, legalMoveDTO) {
    try{

        const lastMove = MadeMoves.length > 0 ? MadeMoves[MadeMoves.length - 1] : null;

        let promotionPiece = "none" // default for promotion

        // this is how to get the name like "wp"
        //console.log(board[move.fromRow][move.fromCol]);

        console.log(legalMoveDTO.isPawnPromotion);

        // board[move.toRow][move.toCol] -> this is the board position of the piece that we want to promote
        if (legalMoveDTO.isPawnPromotion) {
            promotionPiece = await handlePawnPromotion(move.toRow, move.toCol); // we want to send over
            // the position of the end cell so that we could throw the window there and also need color
        }

        console.log(promotionPiece);

        const moveData = {
            gameId: gameId,
            from: move.from,
            to: move.to,
            promotionPiece: promotionPiece
        };


        const response = await fetchAPI.post("MakeMove", moveData);

        if (response.success) {
            board = response.board;
            const originalCell = document.getElementById(move.from);
            const pieceToMove = originalCell.querySelector('.chessPiece');
            
            if (pieceToMove) {
                const existingPiece = targetCell.querySelector('.chessPiece');
                if (existingPiece) {
                    existingPiece.remove();
                }

                // Handle en passant capture
                if (legalMoveDTO.isEnPassant) {
                    handleEnPassantCapture(move);
                }

                // handles promotion
                if (promotionPiece !== "none") 
                {
                    pieceToMove.src = `./assets/${promotionPiece}.png`;
                    pieceToMove.alt = promotionPiece;
                    pieceToMove.dataset.pieceValue = promotionPiece;
                    board[move.toRow][move.toCol] = promotionPiece;
                } 
                else 
                {
                    board[move.toRow][move.toCol] = board[move.fromRow][move.fromCol];
                }

                pieceToMove.style.opacity = '1';
                pieceToMove.dataset.row = move.toRow;
                pieceToMove.dataset.col = move.toCol;
                pieceToMove.dataset.algebraic = move.to;
                
                targetCell.appendChild(pieceToMove);
                
                //board[move.toRow][move.toCol] = board[move.fromRow][move.fromCol];
                board[move.fromRow][move.fromCol] = "-";

                MadeMoves.push(move.from + move.to);
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

function handleEnPassantCapture(move) {
    console.log("enteredenpassanterule")
    const capturedPawnRow = move.fromRow;
    const capturedPawnCol = move.toCol;
    const capturedPawnCell = document.querySelector(`[data-row="${capturedPawnRow}"][data-col="${capturedPawnCol}"]`);
    
    if (capturedPawnCell) {
        const capturedPiece = capturedPawnCell.querySelector('.chessPiece');
        if (capturedPiece) {
            capturedPiece.remove();
            board[capturedPawnRow][capturedPawnCol] = "-";
        }
    }
}

async function updateLegalMoves() {
    try {
        const lastMove = MadeMoves.length > 0 ? MadeMoves[MadeMoves.length - 1] : null;
        LegalMoves = await fetchAPI.post("GetLegalMoves", {
            gameId: gameId,
        });
        console.log("Legal moves:", LegalMoves);
    } catch (error) {
        console.error("Error updating legal moves: " + error);
    }
}

document.addEventListener('dragend', function(e) {
    if (e.target.classList.contains('chessPiece')) {
        e.target.style.opacity = '1';
    }
});

async function handlePawnPromotion(row, col) {
    return new Promise((resolve) => {
        const color = board[row][col].startsWith("w") ? "b" : "w";
        const modal = document.createElement("div");
        modal.className = "promotion-window";

        const cellSize = document.querySelector('.cell').offsetWidth;
        modal.style.top = `${row * cellSize}px`;
        modal.style.left = `${col * cellSize}px`;

        const pieces = ["B", "N", "Q", "R"];
        pieces.forEach(p => {
            const img = document.createElement("img");
            img.src = `./assets/${color + p}.png`;
            img.classList.add("promotion-choice");
            img.onclick = () => {
                document.body.removeChild(modal);
                resolve(color + p);
            };
            modal.appendChild(img);
        });

        const boardRect = document.querySelector('.chessBoard').getBoundingClientRect();
        modal.style.position = "absolute";
        modal.style.top = `${boardRect.top + row * cellSize}px`;
        modal.style.left = `${boardRect.left + col * cellSize}px`;

        document.body.appendChild(modal);
    });
}

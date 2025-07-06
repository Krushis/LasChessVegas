import AnnotationHelper from "./AnnotationHelper.js";
import FetchWrapper from "./ApiWrapper.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");
const annotationHelper = new AnnotationHelper();

let board = null;
let LegalMoves = [];

main();

async function main() {

    await initializeBoard();
    LegalMoves = await fetchAPI.post("GetLegalMoves", board );

}


// give each cell the id of the row and cell it represents
// make each image draggable
// make the dragging so we when we select (start dragging it gets the value of the cell) and drop it
// on a different cell (get that id), if that is in legalmoves, then we allow it

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


                // todo: implement
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

function handleDragStart(e) {
    draggedPiece = e.target;
    draggedFrom = {
        row: parseInt(e.target.dataset.row),
        col: parseInt(e.target.dataset.col),
        piece: e.target.dataset.pieceValue,
        algebraic: e.target.dataset.algebraic
    };

    e.target.style.opacity = '0.5';
    
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', e.target.outerHTML);

}

function handleDragOver(e) {
    e.preventDefault(); // Allow drop
    e.dataTransfer.dropEffect = 'move';
}

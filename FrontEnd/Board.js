import FetchWrapper from "./ApiWrapper.js";
import { setBoard, setGameId, resetGameState } from "./GameState.js";
import AnnotationHelper from "./AnnotationHelper.js";
import { handleDragOver, handleDragStart, handleDrop } from "./DragPieces.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");
const annotationHelper = new AnnotationHelper();

export async function initializeBoardUI() {
    try {
        resetGameState();
        const dto = {
            Player1Id: "test1",
            Player2Id: "test2" 
        };

        const jsonData = await fetchAPI.post("CreateAndGetBoard", dto);
        setBoard(jsonData.board);
        setGameId(jsonData.gameId);

        let board = jsonData.board;

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
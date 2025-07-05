import FetchWrapper from "./ApiWrapper.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");

initializeBoard();

async function initializeBoard() {
    try {
        const jsonData = await fetchAPI.get("initializeAndGetBoard");
        console.log("Hello");
        console.log(jsonData);
        const board = jsonData.board;

        console.log(board[0][0]);

        const boardDiv = document.querySelector(".chessBoard");
        boardDiv.innerHTML = "";

        for(let row = 0; row < 8; row++) {

            const rowDiv = document.createElement("div");
            rowDiv.className = 'row';

            for(let col = 0; col < 8; col++) {

                const cell = document.createElement("div");
                cell.className = "cell";
                
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
                    cell.appendChild(pieceImage);
                }
                
                rowDiv.appendChild(cell);
            }

            boardDiv.appendChild(rowDiv);

        }


    }

    catch(error) {
        console.error("Error trying to initialize board - " + error);
    }
    
};
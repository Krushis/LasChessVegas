import { gameId, incrementMoveCount, board, setBoard, MadeMoves, draggedPiece} from "./GameState.js";
import FetchWrapper from "./ApiWrapper.js";
import {updateLegalMoves, checkEndGame}  from "./ApiLogic.js";
import AnnotationHelper from "./AnnotationHelper.js";

const fetchAPI = new FetchWrapper("http://localhost:5098/");
const annotationHelper = new AnnotationHelper();

export async function executeMove(move, targetCell, legalMoveDTO, pieceToMove) {
    try{
        let promotionPiece = "none" // default for promotion

        // this is how to get the name like "wp"
        //console.log(board[move.fromRow][move.fromCol]);

        //console.log(legalMoveDTO.isPawnPromotion);

        // board[move.toRow][move.toCol] -> this is the board position of the piece that we want to promote
        if (legalMoveDTO.isPawnPromotion) {
            promotionPiece = await handlePawnPromotion(move.toRow, move.toCol); // we want to send over
            // the position of the end cell so that we could throw the window there and also need color
        }

        //console.log(promotionPiece);

        const moveData = {
            gameId: gameId,
            from: move.from,
            to: move.to,
            promotionPiece: promotionPiece
        };


        const response = await fetchAPI.post("MakeMove", moveData);

        if (response.success) {
            setBoard(response.board);
            const originalCell = document.getElementById(move.from);
            pieceToMove = originalCell.querySelector('.chessPiece');
            
            if (pieceToMove) {
                MadeMoves.push(move.from + move.to);
                const existingPiece = targetCell.querySelector('.chessPiece');
                if (existingPiece) {
                    existingPiece.remove();
                }

                if(legalMoveDTO.isCastle) {
                    handleCastling();
                    if (draggedPiece) {
                    draggedPiece.style.opacity = '1';
                    draggedPiece = null;
                    }
                }
                else{
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
                }

                pieceToMove.style.opacity = '1';
                pieceToMove.dataset.row = move.toRow;
                pieceToMove.dataset.col = move.toCol;
                pieceToMove.dataset.algebraic = move.to;
                
                targetCell.appendChild(pieceToMove);
                
                //board[move.toRow][move.toCol] = board[move.fromRow][move.fromCol];
                board[move.fromRow][move.fromCol] = "-";

                //MadeMoves.push(move.from + move.to);
                incrementMoveCount()

                await updateLegalMoves();
                await checkEndGame();
            }
        } else {
            pieceToMove.style.opacity = '1';
            console.log("Move not allowed by the backend, move - " + move.from + move.to);
        }

    }
    catch(error) {
        console.log("Found error in executeMove - " + error);
        pieceToMove.style.opacity = '1';
    }
}

export function handleEnPassantCapture(move) {
    //console.log("enteredenpassanterule")
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

export async function handlePawnPromotion(row, col) {
    return new Promise((resolve) => {
        const color = board[row][col].startsWith("w") ? "b" : "w";
        document.getElementById("overlay-blocker").style.display = "block";
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
                document.getElementById("overlay-blocker").style.display = "none";
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

export function handleCastling()
{
    const lastMove = MadeMoves[MadeMoves.length - 1]; //e1g1
    if (!lastMove) return;

    const from = lastMove.substring(0, 2); // e1 or e8
    const to = lastMove.substring(2, 4);   // g1/g8 or c1/c8

    const isWhite = from[1] === "1";

    let rookFromId, rookToId;

    if (to === "g1" || to === "g8") {
        // Kingside castling
        rookFromId = isWhite ? "h1" : "h8";
        rookToId = isWhite ? "f1" : "f8";
    } else if (to === "c1" || to === "c8") {
        // Queenside castling
        rookFromId = isWhite ? "a1" : "a8";
        rookToId = isWhite ? "d1" : "d8";
    } else {
        console.warn("Unrecognized castling move:", lastMove);
        return;
    }

    const rookFromCell = document.getElementById(rookFromId);
    const rookToCell = document.getElementById(rookToId);

    if (!rookFromCell || !rookToCell) {
        console.warn("Castling rook move failed: missing cell.");
        return;
    }

    const rookPiece = rookFromCell.querySelector('.chessPiece');
    if (!rookPiece) {
        console.warn("No rook found in rookFromCell:", rookFromId);
        return;
    }

    rookFromCell.removeChild(rookPiece);
    rookToCell.appendChild(rookPiece);

    rookPiece.dataset.row = rookToCell.dataset.row;
    rookPiece.dataset.col = rookToCell.dataset.col;
    rookPiece.dataset.algebraic = rookToId;

    const fromCoords = annotationHelper.algebraicToIndex(rookFromId);
    const toCoords = annotationHelper.algebraicToIndex(rookToId);

    board[fromCoords[1]][fromCoords[0]] = "-";
    board[toCoords[1]][toCoords[0]] = rookPiece.dataset.pieceValue;
}
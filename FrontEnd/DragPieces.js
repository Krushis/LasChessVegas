import { draggedPiece, draggedFrom, setDraggedPiece, setDraggedFrom, resetDraggedState, LegalMoves } from "./GameState.js";
import { executeMove } from "./MoveLogic.js";

export function handleDragStart(event) {
    if (document.getElementById("overlay-blocker").style.display === "block") {
        event.preventDefault();
        return;
    }

    event.target.classList.add("highlighted-cell");

    setDraggedPiece(event.target);
    setDraggedFrom({
        row: parseInt(event.target.dataset.row),
        col: parseInt(event.target.dataset.col),
        piece: event.target.dataset.pieceValue,
        algebraic: event.target.dataset.algebraic
    });

    event.target.style.opacity = '0.5';
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/html', event.target.outerHTML);
}

export function handleDragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
}

export function handleDrop(event) {
    if (document.getElementById("overlay-blocker").style.display === "block") {
        return;
    }

    event.preventDefault();

    if (!draggedPiece || !draggedFrom) return;

    const dropCell = event.target.classList.contains('cell') 
        ? event.target 
        : event.target.closest('.cell');
    if (!dropCell) return;

    const move = {
        from: draggedFrom.algebraic,
        to: dropCell.id,
        fromRow: draggedFrom.row,
        fromCol: draggedFrom.col,
        toRow: parseInt(dropCell.dataset.row),
        toCol: parseInt(dropCell.dataset.col)
    };

    const moveString = move.from + move.to;
    const legalMoveDTO = LegalMoves.find(legalMove => legalMove.move === moveString);

    if (legalMoveDTO) {
        executeMove(move, dropCell, legalMoveDTO, draggedPiece);
    } else {
        draggedPiece.style.opacity = '1';
        console.log("Illegal move:", moveString);
    }

    resetDraggedState();
}

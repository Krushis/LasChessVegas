export let board = null;
export let gameId = null;
export let LegalMoves = [];
export let MadeMoves = [];
export let moveCount = 0;
export let draggedPiece = null;
export let draggedFrom = null;

export function setBoard(newBoard) {
    board = newBoard;
}

export function setGameId(newGameId) {
    gameId = newGameId;
}

export function setLegalMoves(moves) {
    LegalMoves = moves;
}

export function addMadeMove(move) {
    MadeMoves.push(move);
}

export function setMadeMoves(moves) {
    MadeMoves = moves;
}

export function setMoveCount(count) {
    moveCount = count;
}

export function incrementMoveCount() {
    moveCount++;
}

export function setDraggedPiece(piece) {
    draggedPiece = piece;
}

export function setDraggedFrom(data) {
    draggedFrom = data;
}

export function resetDraggedState() {
    draggedPiece = null;
    draggedFrom = null;
}

export function resetGameState() {
    board = null;
    gameId = null;
    LegalMoves = [];
    MadeMoves = [];
    moveCount = 0;
}

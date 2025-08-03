export default class AnnotationHelper{
    constructor() {

    }

    indexToAlgebraic(col, row) {
        const file = String.fromCharCode('a'.charCodeAt(0) + col);
        const rank = 8 - row;
        return `${file}${rank}`;
    }

    makeAnnotation(startCol, startRow, endCol, endRow) {
        const start = this.indexToAlgebraic(startCol, startRow);
        const end = this.indexToAlgebraic(endCol, endRow);
        return start + end;
    }

    algebraicToIndex(square) {
    if (!square || square.length !== 2) return null;

    const file = square[0].toLowerCase();
    const rank = square[1];

    if (file < 'a' || file > 'h' || rank < '1' || rank > '8') return null;

    const col = file.charCodeAt(0) - 'a'.charCodeAt(0);
    const row = 8 - parseInt(rank, 10);

    return [row, col]; // Return as array for [row, col]
    }

}

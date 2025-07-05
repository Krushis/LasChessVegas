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
}
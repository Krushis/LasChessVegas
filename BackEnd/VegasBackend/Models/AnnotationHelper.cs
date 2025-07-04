using VegasBackend.Models.Pieces;

namespace VegasBackend.Models
{
    public class AnnotationHelper
    {

        /// <summary>
        /// Helps make the Annotation for the move we want to make
        /// </summary>
        /// <param name="sCol"></param>
        /// <param name="sRow"></param>
        /// <param name="eCol"></param>
        /// <param name="eRow"></param>
        /// <returns>a string like, e4e5 (starting row is 4) for the annotation of the move we want to make</returns>
        public static string MakeMove(int sCol, int sRow, int eCol, int eRow)
        {
            string start = IndexToAlgebraic(sCol, sRow);
            string end = IndexToAlgebraic(eCol, eRow);
            return start + end;
        }

        private static string IndexToAlgebraic(int col, int row)
        {
            char file = (char)('a' + col);
            int rank = 8 - row;
            return $"{file}{rank}";
        }
    }
}

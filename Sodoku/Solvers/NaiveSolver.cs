using System.Diagnostics;

namespace Sodoku.Solvers
{
    public sealed class NaiveSolver: ISolver
    {
        public bool TrySolve(Board board)
        {
            Debug.Assert(board.TryGetNextEmptyCell(
                currentEmptyCell: null,
                nextEmptyCell: out var firstEmptyCell
            ));

            return TrySolveCell(board, firstEmptyCell);
        }

        private static bool TrySolveCell(Board board, Cell currentEmptyCell)
        {
            for (int val = Constants.MIN_VALUE; val <= Constants.MAX_VALUE; val++)
            {
                if (!currentEmptyCell.SetValueIfValid(board, val))
                {
                    continue;
                }

                if (!board.TryGetNextEmptyCell(currentEmptyCell, out var nextEmptyCell))
                {
                    return true;
                }

                if (TrySolveCell(board, nextEmptyCell))
                {
                    return true;
                }
            }

            // Delete and backtrack

            currentEmptyCell.ClearValue();

            return false;
        }
    }
}
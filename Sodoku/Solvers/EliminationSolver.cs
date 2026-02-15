using System.Diagnostics;

namespace Sodoku.Solvers
{
    public sealed class EliminationSolver: ISolver
    {
        private readonly struct ValidCellValues
        {
            private static void SetValidState(ref ushort valuesBitMask, int value)
            {
                // E.x. Value 1 will be stored at index 0 from LSB
                var zeroBasedIndex = value - 1;

                valuesBitMask |= unchecked((ushort) (1 << zeroBasedIndex));
            }

            private readonly ushort ValuesBitMask;

            public ValidCellValues(IList<int> validValues)
            {
                ushort valuesBitMask = 0;

                foreach (var validValue in validValues)
                {
                    SetValidState(ref valuesBitMask, validValue);
                }

                ValuesBitMask = valuesBitMask;
            }

            public bool this[int value]
            {
                get
                {
                    var zeroBasedIndex = value - 1;

                    return (ValuesBitMask & (1 << zeroBasedIndex)) != 0;
                }
            }
        }

        private static ref ValidCellValues GetValidCellValuesForCellSlotRef(ValidCellValues[] validCellValues, Cell cell)
        {
            return ref validCellValues[cell.Index];
        }

        public bool TrySolve(Board board)
        {
            var validCellValues = new ValidCellValues[Constants.BOARD_SIZE];

            board.MarkNewEmptyCells(updatedEmptyCells =>
            {
                var validValues = new List<int>();

                // Operate on a copy so we can remove empty cells as we iterate
                foreach (var currentEmptyCell in updatedEmptyCells.ToArray())
                {
                    for (int value = Constants.MIN_VALUE; value <= Constants.MAX_VALUE; value++)
                    {
                        // Maybe we should have a separate API for checking the validity of a value for a given cell...
                        if (currentEmptyCell.SetValueIfValid(board, value))
                        {
                            validValues.Add(value);
                        }
                    }

                    var existingValuesCount = validValues.Count;

                    if (existingValuesCount > 1)
                    {
                        // Clear value, we are actually not trying to fill it
                        currentEmptyCell.ClearValue();

                        GetValidCellValuesForCellSlotRef(validCellValues, currentEmptyCell) = new(
                            validValues
                        );
                    }

                    else
                    {
                        Debug.Assert(existingValuesCount != 0);

                        // No need to clear set value of empty cell -
                        // Since only a single value is possible, we set said value and convert it to a filled cell

                        updatedEmptyCells.Remove(currentEmptyCell);
                    }

                    validValues.Clear();
                }
            });

            Debug.Assert(board.TryGetNextEmptyCell(
                currentEmptyCell: null,
                nextEmptyCell: out var firstEmptyCell
            ));

            return TrySolveCell(board, firstEmptyCell, validCellValues);
        }

        private static bool TrySolveCell(Board board, Cell currentEmptyCell, ValidCellValues[] validCellValues)
        {
            var currentCellValidValues = GetValidCellValuesForCellSlotRef(validCellValues, currentEmptyCell);

            for (int val = Constants.MIN_VALUE; val <= Constants.MAX_VALUE; val++)
            {
                if (!currentCellValidValues[val])
                {
                    continue;
                }

                if (!currentEmptyCell.SetValueIfValid(board, val))
                {
                    continue;
                }

                if (!board.TryGetNextEmptyCell(currentEmptyCell, out var nextEmptyCell))
                {
                    return true;
                }

                if (TrySolveCell(board, nextEmptyCell, validCellValues))
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
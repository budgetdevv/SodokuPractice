using System.Diagnostics.CodeAnalysis;

namespace Sodoku
{
    public sealed class Board
    {
        private readonly Cell[] Cells;

        private readonly Cell[] EmptyCells;

        public Board(int?[,] cellValues)
        {
            var cells = Cells = new Cell[Constants.BOARD_SIZE];

            var emptyCells = new List<Cell>(Constants.BOARD_SIZE);

            for (var row = 0; row < Constants.ROW_OR_COLUMN_SIZE; row++)
            {
                for (var col = 0; col < Constants.ROW_OR_COLUMN_SIZE; col++)
                {
                    var value = cellValues[row, col];

                    var newCell = new Cell(row, col, value);

                    cells[newCell.Index] = newCell;

                    if (value == null)
                    {
                        emptyCells.Add(newCell);
                    }
                }
            }
            EmptyCells = emptyCells.ToArray();
        }

        public ReadOnlySpan<Cell> GetRow(Cell cell)
        {
            var rowIndex = cell.Row;

            return Cells.AsSpan(
                rowIndex * Constants.ROW_OR_COLUMN_SIZE,
                Constants.ROW_OR_COLUMN_SIZE
            );
        }

        public Cell[] GetColumn(Cell cell)
        {
            var colIndex = cell.Column;

            var columnCells = new Cell[Constants.ROW_OR_COLUMN_SIZE];

            for (var rowIndex = 0; rowIndex < Constants.ROW_OR_COLUMN_SIZE; rowIndex++)
            {
                columnCells[rowIndex] = Cells[Helpers.ComputeCellIndex(rowIndex, colIndex)];
            }

            return columnCells;
        }

        public Cell[] GetBoxCells(Cell cell)
        {
            var rowIndex = cell.Row;

            var colIndex = cell.Column;

            const int BOX_LENGTH = Constants.BOX_WIDTH_OR_LENGTH;

            var startingRowIndex = rowIndex - (rowIndex % BOX_LENGTH);

            var startingColIndex = colIndex - (colIndex % BOX_LENGTH);

            var boxCells = new Cell[Constants.BOX_SIZE];

            var boxCellCount = 0;

            for (int boxRowIndex = startingRowIndex; boxRowIndex < startingRowIndex + BOX_LENGTH; boxRowIndex++)
            {
                for (int boxColIndex = startingColIndex; boxColIndex < startingColIndex + BOX_LENGTH; boxColIndex++)
                {
                    var currentCell = Cells[Helpers.ComputeCellIndex(boxRowIndex, boxColIndex)];

                    boxCells[boxCellCount++] = currentCell;
                }
            }

            return boxCells;
        }

        public bool TryGetNextEmptyCell(Cell? currentEmptyCell, [NotNullWhen(returnValue: true)] out Cell? nextEmptyCell)
        {
            var emptyCells = EmptyCells;

            if (currentEmptyCell == null)
            {
                nextEmptyCell = emptyCells[0];

                return true;
            }

            if (currentEmptyCell.IsEmpty)
            {
                throw new InvalidOperationException("The current empty cell must not have a value assigned to it.");
            }

            var currentEmptyCellIndex = emptyCells.IndexOf(currentEmptyCell);

            if (currentEmptyCellIndex == -1)
            {
                throw new InvalidOperationException("The current empty cell must be part of the board's empty cells.");
            }

            var indexOfLastEmptyCell = emptyCells.Length - 1;

            if (currentEmptyCellIndex != indexOfLastEmptyCell)
            {
                nextEmptyCell = emptyCells[currentEmptyCellIndex + 1];

                return true;
            }

            nextEmptyCell = null;

            return false;
        }

        public override string ToString()
        {
            var lines = new List<string>();

            for (var row = 0; row < Constants.ROW_OR_COLUMN_SIZE; row++)
            {
                var lineCells = new List<string>();

                for (var col = 0; col < Constants.ROW_OR_COLUMN_SIZE; col++)
                {
                    var cell = Cells[Helpers.ComputeCellIndex(row, col)];

                    lineCells.Add(cell.ToString());
                }

                lines.Add(string.Join(" ", lineCells));
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
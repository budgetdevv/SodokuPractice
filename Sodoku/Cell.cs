namespace Sodoku
{
    public sealed class Cell(int row, int column, int? originalValue)
    {
        public readonly int Row = row;

        public readonly int Column = column;

        public readonly int Index = row * Constants.ROW_OR_COLUMN_SIZE + column;

        private int? Value = originalValue;

        public bool IsEmpty => Value == null;

        public bool SetValueIfValid(Board board, int value)
        {
            foreach (var cell in board.GetRow(this))
            {
                if (cell.Value == value)
                {
                    return false;
                }
            }

            foreach (var cell in board.GetColumn(this))
            {
                if (cell.Value == value)
                {
                    return false;
                }
            }

            foreach (var cell in board.GetBoxCells(this))
            {
                if (cell.Value == value)
                {
                    return false;
                }
            }

            Value = value;

            return true;
        }

        public void ClearValue()
        {
            Value = null;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? ".";
        }
    }
}
namespace Sodoku
{
    public static class Helpers
    {
        public static int ComputeCellIndex(int rowIndex, int columnIndex)
        {
            return (rowIndex * Constants.ROW_OR_COLUMN_SIZE) + columnIndex;
        }
    }
}
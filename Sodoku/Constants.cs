namespace Sodoku
{
    public static class Constants
    {
        public const int
            ROW_OR_COLUMN_SIZE = 9,
            BOX_WIDTH_OR_LENGTH = 3,
            BOX_SIZE = BOX_WIDTH_OR_LENGTH * BOX_WIDTH_OR_LENGTH,
            BOARD_SIZE = ROW_OR_COLUMN_SIZE * ROW_OR_COLUMN_SIZE,
            MIN_VALUE = 1,
            MAX_VALUE = 9;
    }
}
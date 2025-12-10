using System;

namespace CSVReader.Utilities
{
    /// <summary>
    /// Stores information about a DataGrid cell (row, column, value)
    /// </summary>
    public sealed class CellInfo
    {
        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public string Value { get; }

        /// <summary>
        /// Creates a new cell info object with position and value
        /// </summary>
        public CellInfo(int rowIndex, int columnIndex, string value)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Value = value ?? string.Empty;
        }

        /// <summary>
        /// Returns the cell value as string
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Generates a display title showing cell position (e.g., "Cell [1,2]")
        /// </summary>
        public string Title
        {
            get
            {
                if (RowIndex >= 0 && ColumnIndex >= 0)
                    return $"Cell [{RowIndex + 1},{ColumnIndex + 1}]";
                return "Cell";
            }
        }
    }
}
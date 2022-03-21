using QuickParser.Attributes;
using QuickParser.Helpers;

namespace QuickParser.Classes
{
    /// <summary>
    /// Contains all the cells content for a single row
    /// </summary>
    public class ParsedRow
    {
        private readonly IDictionary<string, string> _cells;

        public ParsedRow(IDictionary<string, string> cells)
        {
            _cells = cells;
        }

        /// <summary>
        /// Gets the value of the cell at a specified index
        /// </summary>
        /// <param name="columnIndex">Index of the cell to get the value of</param>
        /// <returns>Value of the cell at a specified index</returns>
        public string this[int columnIndex] => columnIndex < _cells.Count ? _cells.ElementAt(columnIndex).Value : "";

        /// <summary>
        /// Gets the value of the cell in a specified column
        /// </summary>
        /// <param name="columnName">Name of the column to look in</param>
        /// <returns>Value of the cell in a specified column or an empty string for invalid column names</returns>
        public string this[string columnName] => _cells.ContainsKey(columnName) ? _cells[columnName] : "";

        /// <summary>
        /// Gets the value of the cell in a specified column
        /// </summary>
        /// <param name="columnName">Enum member that has a <see cref="ColumnAttribute"/></param>
        /// <returns>Value of the cell in a specified column or an empty string for invalid columns</returns>
        public string this[Enum columnName] => this[columnName.GetAttributeOfType<ColumnAttribute>()?.Name ?? ""];
    }
}
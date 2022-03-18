﻿namespace QuickParser.Classes
{
    /// <summary>
    /// Contains all the cells content for a single row
    /// </summary>
    public class ParsedRow
    {
        private readonly Dictionary<string, string> _cells;

        public ParsedRow(Dictionary<string, string> cells)
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
        /// <returns>Value of the cell in a specified column</returns>
        public string this[string columnName] => _cells.ContainsKey(columnName) ? _cells[columnName] : "";
    }
}
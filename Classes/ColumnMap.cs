using QuickParser.Attributes;
using QuickParser.Helpers;

namespace QuickParser.Classes
{
    /// <summary>
    /// Contains transform instructions to enable the reading and conversion of the cells of a column
    /// </summary>
    /// <typeparam name="T">Type of the transformed value</typeparam>
    public class ColumnMap<T>
    {
        private readonly Func<ParsedRow, T>? _transformWithoutValue = null;
        private readonly Func<string?, ParsedRow, T>? _transformWithValue = null;

        /// <summary>
        /// Name of the column
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Index of the column (zero based)
        /// </summary>
        public int? Index { get; set; }

        public ColumnMap(Enum columnName, Func<string?, T> transform)
            : this(columnName, (value, _) => transform.Invoke(value))
        {

        }

        public ColumnMap(int columnIndex, Func<string?, T> transform)
            : this(columnIndex, (value, _) => transform.Invoke(value))
        {

        }

        public ColumnMap(Enum columnName, Func<string?, ParsedRow, T> transform)
        {
            Name = columnName.GetAttributeOfType<ColumnAttribute>()?.Name ?? "";
            _transformWithValue = transform;
        }

        public ColumnMap(int columnIndex, Func<string?, ParsedRow, T> transform)
        {
            Index = columnIndex;
            _transformWithValue = transform;
        }

        public ColumnMap(Func<ParsedRow, T> transform)
        {
            _transformWithoutValue = transform;
        }

        public ColumnMap(string columnName)
        {
            Name = columnName;
        }

        public ColumnMap(Enum columnName)
        {
            Name = columnName.GetAttributeOfType<ColumnAttribute>()?.Name ?? "";
        }

        public ColumnMap(int columnIndex)
        {
            Index = columnIndex;
        }

        /// <summary>
        /// Gets and applies transforms and returns the value of this column for a specified row
        /// </summary>
        /// <param name="row">Row from which to get the value</param>
        /// <returns>Value of this column for the specified row</returns>
        public T? GetValue(ParsedRow row)
        {
            if (_transformWithoutValue != null)
                return _transformWithoutValue.Invoke(row);
            else if (_transformWithValue != null)
                return _transformWithValue.Invoke(Index.HasValue ? row[Index.Value] : (!string.IsNullOrEmpty(Name) ? row[Name] : default), row);
            else if (typeof(T) == typeof(string))
                return Index.HasValue ? (T)(object)row[Index.Value] : (!string.IsNullOrEmpty(Name) ? (T)(object)row[Name] : default);

            return default;
        }
    }
}
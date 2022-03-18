using QuickParser.Interfaces;

namespace QuickParser.Classes
{
    /// <summary>
    /// Provides the necessary structure to map CSV content to C# objects
    /// </summary>
    /// <typeparam name="TObject">Type of the resulting C# object</typeparam>
    public abstract class ParserBase<TObject>
    {
        protected List<ParsedRow> _parsedRows;
        protected string[] _columns;

        /// <summary>
        /// Overwrite when the delimiter character isn't a comma
        /// </summary>
        protected virtual string Delimiter { get; } = ",";

        /// <summary>
        /// Provides all column names
        /// </summary>
        protected abstract string[] Headers { get; }

        /// <summary>
        /// Provides the mapping recipe for all properties
        /// </summary>
        protected abstract IParsedRowMapping<TObject> Mapping { get; }

        /// <summary>
        /// Row index at where the header is located (zero based)
        /// </summary>
        protected virtual int HeaderRowIndex { get; } = 0;

        public ParserBase(string csv)
        {
            string[] lines = csv.Split('\n');

            _columns = lines[HeaderRowIndex].Split(Delimiter).Select(c => c.Trim().Replace("\"", "")).ToArray();

            if (!IsHeaderValid())
                throw new Exception("Unrecognized header format.");

            _parsedRows = lines.Skip(HeaderRowIndex + 1)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(line => new ParsedRow(line.Split(Delimiter)
                    .Select((c, i) => new { ColumnName = _columns[i], Value = c.StartsWith("\"") && c.EndsWith("\"") ? c[1..^1] : c })
                    .ToDictionary(c => c.ColumnName, c => c.Value)))
                .ToList();
        }

        private bool IsHeaderValid()
        {
            if (Headers.Length != _columns.Length)
                return false;

            for (int i = 0; i < Headers.Length; i++)
                if (Headers[i] != _columns[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Overwrite to edit the resulting list of <see cref="TObject"/> before it is returned by <see cref="Parse"/>
        /// </summary>
        /// <param name="objects">Unedited parsed <see cref="IEnumerable{TObject}"/> of <see cref="TObject"/></param>
        /// <returns>Final list of <see cref="TObject"/></returns>
        protected virtual List<TObject> PostProcessing(IEnumerable<TObject> objects) => objects.ToList();

        /// <summary>
        /// Overwrite to modify or augment the conversion between parsed rows and the resulting C# objects
        /// </summary>
        protected virtual IEnumerable<TObject> OnParse => _parsedRows.Select(r => Mapping.Map(r, GetParams()));

        /// <summary>
        /// Overwrite to provide extra data to the <see cref="IParsedRowMapping{T}.Map"/> method
        /// </summary>
        /// <returns>All objects that <see cref="IParsedRowMapping{T}.Map"/> should receive</returns>
        protected virtual object[] GetParams() => Array.Empty<object>();

        /// <summary>
        /// Converts all <see cref="ParsedRow"/> into <see cref="TObject"/>, runs <see cref="PostProcessing(IEnumerable{TObject})"/> and returns
        /// </summary>
        /// <returns>Final list of parsed objects</returns>
        public List<TObject> Parse() => PostProcessing(OnParse);
    }
}
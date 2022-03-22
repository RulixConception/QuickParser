using QuickParser.Attributes;
using QuickParser.Helpers;
using QuickParser.Interfaces;

namespace QuickParser.Classes
{
    /// <summary>
    /// Provides the necessary structure to map CSV content to C# objects
    /// </summary>
    /// <typeparam name="TEntity">Type of the resulting C# object</typeparam>
    /// <typeparam name="TColumnDef">Type of the enum that defines the columns of the CSV</typeparam>
    public abstract class ParserBase<TEntity, TColumnDef> : IParserBase<TEntity> where TColumnDef : struct, IConvertible
    {
        private readonly IList<ParsedRow> _parsedRows;
        private readonly string[] _columns;

        /// <summary>
        /// Overwrite when the delimiter character isn't a comma
        /// </summary>
        protected virtual string Delimiter { get; } = ",";

        /// <summary>
        /// Provides all column names
        /// </summary>
        protected string[] _headers = Enum.GetValues(typeof(TColumnDef))
            .OfType<TColumnDef>()
            .Select(v => (v as Enum)?.GetAttributeOfType<ColumnAttribute>()?.Name ?? "")
            .ToArray();

        /// <summary>
        /// Provides the mapping recipe for all properties
        /// </summary>
        protected abstract IParsedRowMapping<TEntity> Mapping { get; }

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
            if (_headers.Length != _columns.Length)
                return false;

            for (int i = 0; i < _headers.Length; i++)
                if (_headers[i] != _columns[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Overwrite to edit the resulting list of <see cref="TEntity"/> before it is returned by <see cref="Parse"/>
        /// </summary>
        /// <param name="objects">Unedited parsed <see cref="IEnumerable{TObject}"/> of <see cref="TEntity"/></param>
        /// <returns>Final list of <see cref="TEntity"/></returns>
        protected virtual IList<TEntity> PostProcessing(IEnumerable<TEntity> objects) => objects.ToList();

        /// <summary>
        /// Overwrite to modify or augment the conversion between parsed rows and the resulting C# objects
        /// </summary>
        protected virtual IEnumerable<TEntity> OnParse() => _parsedRows.Select(r => Mapping.Map(r, OnInstantiate()));

        /// <summary>
        /// Creates and returns an instance of <see cref="TEntity"/>
        /// </summary>
        /// <param name="parameters">Optional parameters provided by overwritting <see cref="GetParams"/></param>
        /// <returns>An instance of <see cref="TEntity"/></returns>
        protected virtual TEntity OnInstantiate() => (TEntity?)Activator.CreateInstance(typeof(TEntity)) ?? throw new NullReferenceException();

        /// <summary>
        /// Converts all <see cref="ParsedRow"/> into <see cref="TEntity"/>, runs <see cref="PostProcessing(IEnumerable{TEntity})"/> and returns
        /// </summary>
        /// <returns>Final list of parsed objects</returns>
        public IList<TEntity> Parse() => PostProcessing(OnParse());

        /// <summary>
        /// Same as <see cref="Parse"/> but will cast into type <see cref="T"/>
        /// </summary>
        /// <returns>Final list of parsed objects casted into <see cref="T"/></returns>
        public IList<T> Parse<T>() => Parse().Cast<T>().ToList();
    }
}
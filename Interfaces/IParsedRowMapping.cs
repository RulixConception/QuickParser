using QuickParser.Classes;

namespace QuickParser.Interfaces
{
    /// <summary>
    /// Defines what a mapping recipe should implement
    /// </summary>
    /// <typeparam name="TObject">Type of the parsed objects</typeparam>
    public interface IParsedRowMapping<TObject>
    {
        public TObject Map(ParsedRow row, TObject instance, params object[] data);
    }
}
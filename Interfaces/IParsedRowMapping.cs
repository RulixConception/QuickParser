using QuickParser.Classes;

namespace QuickParser.Interfaces
{
    /// <summary>
    /// Defines what a mapping recipe should implement
    /// </summary>
    /// <typeparam name="T">Type of the parsed objects</typeparam>
    public interface IParsedRowMapping<T>
    {
        public T Map(ParsedRow row, params object[] data);
    }
}
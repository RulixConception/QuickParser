namespace QuickParser.Interfaces
{
    public interface IParserBase<TObject>
    {
        IList<TObject> Parse();
    }
}
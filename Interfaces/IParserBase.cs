namespace QuickParser.Interfaces
{
    public interface IParserBase<TObject>
    {
        List<TObject> Parse();
    }
}
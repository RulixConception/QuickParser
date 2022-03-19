namespace QuickParser.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
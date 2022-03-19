namespace QuickParser.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        public static T? GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var attributes = enumVal.GetType().GetMember(enumVal.ToString())[0].GetCustomAttributes(typeof(T), false);

            return attributes?.Length > 0 ? attributes?[0] as T : null;
        }
    }
}
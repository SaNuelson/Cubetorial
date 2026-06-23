using System;

namespace Cubetorial.Odin.Attributes
{
    public class ParsedListAttribute : Attribute
    {
        public readonly string Separator;

        public ParsedListAttribute(string separator = " ")
        {
            Separator = separator;
        }
    }
}
namespace Ds4.Core.Parser;

public sealed class ParseException : Exception
{
    public ParseException(string message) : base(message) { }
}

using Ds4.Core.Formula;

namespace Ds4.Core.Parser;

public sealed class FormulaParser
{
    private readonly List<Token> _tokens;
    private int _pos;

    private FormulaParser(string text)
    {
        _tokens = Tokenize(text);
    }

    public static IFormula Parse(string text)
    {
        var parser = new FormulaParser(text);
        var formula = parser.ParseIff();
        parser.Expect(TokenKind.End);
        return formula;
    }

    private IFormula ParseIff()
    {
        var left = ParseImplies();
        while (Match(TokenKind.Iff))
        {
            var right = ParseImplies();
            left = new Iff(left, right);
        }
        return left;
    }

    private IFormula ParseImplies()
    {
        var left = ParseOr();
        if (Match(TokenKind.Implies))
        {
            var right = ParseImplies();
            return new Implies(left, right);
        }
        return left;
    }

    private IFormula ParseOr()
    {
        var left = ParseAnd();
        while (Match(TokenKind.Or))
            left = new Or(left, ParseAnd());
        return left;
    }

    private IFormula ParseAnd()
    {
        var left = ParseUnary();
        while (Match(TokenKind.And))
            left = new And(left, ParseUnary());
        return left;
    }

    private IFormula ParseUnary()
    {
        if (Match(TokenKind.Not)) return new Not(ParseUnary());
        if (Match(TokenKind.LParen))
        {
            var inner = ParseIff();
            Expect(TokenKind.RParen);
            return inner;
        }
        if (Peek().Kind == TokenKind.Identifier)
        {
            var name = Next().Text;
            if (name.Equals("true", StringComparison.OrdinalIgnoreCase) || name == "⊤") return FormulaTools.True;
            if (name.Equals("false", StringComparison.OrdinalIgnoreCase) || name == "⊥") return FormulaTools.False;
            return new Atom(name);
        }
        throw new ParseException($"Expected formula near '{Peek().Text}'.");
    }

    private bool Match(TokenKind kind)
    {
        if (Peek().Kind != kind) return false;
        _pos++;
        return true;
    }

    private Token Expect(TokenKind kind)
    {
        var token = Peek();
        if (token.Kind != kind) throw new ParseException($"Expected {kind}, got '{token.Text}'.");
        _pos++;
        return token;
    }

    private Token Next() => _tokens[_pos++];
    private Token Peek() => _tokens[_pos];

    private static List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        for (var i = 0; i < text.Length;)
        {
            var c = text[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }
            if (c == '(') { tokens.Add(new(TokenKind.LParen, "(")); i++; continue; }
            if (c == ')') { tokens.Add(new(TokenKind.RParen, ")")); i++; continue; }
            if (c == '&' || c == '∧') { tokens.Add(new(TokenKind.And, c.ToString())); i++; continue; }
            if (c == '|' || c == '∨') { tokens.Add(new(TokenKind.Or, c.ToString())); i++; continue; }
            if (c == '!' || c == '~' || c == '¬') { tokens.Add(new(TokenKind.Not, c.ToString())); i++; continue; }
            if (c == '-' && i + 1 < text.Length && text[i + 1] == '>') { tokens.Add(new(TokenKind.Implies, "->")); i += 2; continue; }
            if (c == '<' && i + 2 < text.Length && text[i + 1] == '-' && text[i + 2] == '>') { tokens.Add(new(TokenKind.Iff, "<->")); i += 3; continue; }
            if (char.IsLetter(c) || c == '_' || c == '⊤' || c == '⊥')
            {
                var start = i;
                i++;
                while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || text[i] == '-')) i++;
                var word = text[start..i];
                tokens.Add(word.ToLowerInvariant() switch
                {
                    "and" => new Token(TokenKind.And, word),
                    "or" => new Token(TokenKind.Or, word),
                    "not" => new Token(TokenKind.Not, word),
                    _ => new Token(TokenKind.Identifier, word)
                });
                continue;
            }
            throw new ParseException($"Unknown character '{c}' in formula.");
        }
        tokens.Add(new(TokenKind.End, "<end>"));
        return tokens;
    }

    private sealed record Token(TokenKind Kind, string Text);
    private enum TokenKind { Identifier, LParen, RParen, Not, And, Or, Implies, Iff, End }
}

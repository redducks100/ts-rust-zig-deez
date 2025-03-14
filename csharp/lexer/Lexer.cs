﻿namespace monkey;

public struct Lexer
{
    private int _position;
    private int _read_position;
    private char _current_char;
    private readonly ReadOnlyMemory<char> _input;

    private ReadOnlySpan<char> _inputSpan { get { return _input.Span; } }

    public Lexer(string input)
    {
        _position = 0;
        _read_position = 0;
        _current_char = '\0';
        _input = input.AsMemory();

        ReadChar();
    }

    public IEnumerable<TokenInfo> ParseTokens()
    {
        TokenInfo tok;
        while ((tok = NextToken()).Type != Token.Eof)
        {
            yield return tok;
        }

        yield return tok;
    }

    public TokenInfo NextToken()
    {
        SkipWhitespace();

        if (_current_char == '\"')
        {
            return new TokenInfo(Token.String, ReadString().ToString());
        }

        if (char.IsLetter(_current_char) || _current_char == '_')
        {
            var ident = ReadIdent();
            return ident switch
            {
                "fn" => new TokenInfo(Token.Function),
                "let" => new TokenInfo(Token.Let),
                "true" => new TokenInfo(Token.True),
                "false" => new TokenInfo(Token.False),
                "if" => new TokenInfo(Token.If),
                "else" => new TokenInfo(Token.Else),
                "return" => new TokenInfo(Token.Return),
                _ => new TokenInfo(Token.Ident, ident.ToString())
            };
        }

        if (char.IsDigit(_current_char))
        {
            return new TokenInfo(Token.Integer, ReadInt());
        }

        var token = _current_char switch
        {
            '=' => Peek() switch
            {
                '=' => SkipAndReturn(new TokenInfo(Token.EQ)),
                _ => new TokenInfo(Token.Assign),
            }
            ,
            '!' => Peek() switch
            {
                '=' => SkipAndReturn(new TokenInfo(Token.NOT_EQ)),
                _ => new TokenInfo(Token.Bang),
            },
            '-' => new TokenInfo(Token.Minus),
            '/' => new TokenInfo(Token.Slash),
            '*' => new TokenInfo(Token.Asterisk),
            '<' => new TokenInfo(Token.LT),
            '>' => new TokenInfo(Token.GT),
            '+' => new TokenInfo(Token.Plus),
            ',' => new TokenInfo(Token.Comma),
            ';' => new TokenInfo(Token.Semicolon),
            '(' => new TokenInfo(Token.LParen),
            ')' => new TokenInfo(Token.RParen),
            '{' => new TokenInfo(Token.LSquirly),
            '}' => new TokenInfo(Token.RSquirly),
            '\0' => new TokenInfo(Token.Eof),
            _ => new TokenInfo(Token.Illegal, _current_char),
        };

        ReadChar();

        return token;
    }


    private void ReadChar()
    {
        if (_read_position >= _input.Length)
        {
            _current_char = '\0';
        }
        else
        {
            _current_char = _inputSpan[_read_position];
        }

        _position = _read_position;
        _read_position++;
    }

    private char Peek()
    {
        if (_read_position >= _input.Length)
        {
            return '\0';
        }
        else
        {
            return _inputSpan[_read_position];
        }
    }

    private void SkipWhitespace()
    {
        ReadWhile(char.IsWhiteSpace);
    }

    private ReadOnlySpan<char> ReadIdent()
    {
        var start = _position;

        ReadWhile(c => char.IsLetter(c) || c == '_');

        return _inputSpan[start.._position];
    }

    private ReadOnlySpan<char> ReadString()
    {
        if (_current_char == '\"')
        {
            ReadChar();
        }

        var start = _position;

        ReadWhile(c => c != '\0' && c != '\"');

        if (_current_char == '\0')
        {
            throw new Exception("Unterminated string literal.");
        }

        var stringValue = _inputSpan[start.._position];

        ReadChar();

        return stringValue;
    }

    private void ReadWhile(Func<char, bool> predicate)
    {
        while (predicate(_current_char))
        {
            ReadChar();
        }
    }

    private int ReadInt()
    {
        var start = _position;

        ReadWhile(char.IsDigit);

        return int.Parse(_inputSpan[start.._position]);
    }

    private TokenInfo SkipAndReturn(TokenInfo tokenInfo)
    {
        ReadChar();
        return tokenInfo;
    }
}
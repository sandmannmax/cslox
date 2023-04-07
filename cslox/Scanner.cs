using System.Globalization;

namespace cslox;

public class Scanner
{
  private readonly string _code;
  private readonly List<Token> _tokens;
  private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
  {
    {"and", TokenType.AND},
    {"class", TokenType.CLASS},
    {"else", TokenType.ELSE},
    {"false", TokenType.FALSE},
    {"for", TokenType.FOR},
    {"fun", TokenType.FUN},
    {"if", TokenType.IF},
    {"nil", TokenType.NIL},
    {"or", TokenType.OR},
    {"print", TokenType.PRINT},
    {"return", TokenType.RETURN},
    {"super", TokenType.SUPER},
    {"this", TokenType.THIS},
    {"true", TokenType.TRUE},
    {"var", TokenType.VAR},
    {"while", TokenType.WHILE},
  };

  private bool _scanned = false;
  private int _current = 0;
  private int _start = 0;
  private int _line = 1;
  
  public Scanner(string code)
  {
    _code = code;
    _tokens = new List<Token>();
  }

  public List<Token> ScanTokens()
  {
    Scan();
    return _tokens;
  }

  private void Scan()
  {
    if (_scanned) return;

    while (!IsAtEnd())
    {
      _start = _current;
      ScanToken();
    }
    
    _tokens.Add(new Token(TokenType.EOF, "", null, _line));
    
    _scanned = true;
  }

  private void ScanToken()
  {
    char c = Advance();
    switch (c)
    {
      case '(':
        AddToken(TokenType.LEFT_PAREN);
        break;
      case ')':
        AddToken(TokenType.RIGHT_PAREN);
        break;
      case '{':
        AddToken(TokenType.LEFT_BRACE);
        break;
      case '}':
        AddToken(TokenType.RIGHT_BRACE);
        break;
      case ',':
        AddToken(TokenType.COMMA);
        break;
      case '.':
        AddToken(TokenType.DOT);
        break;
      case '+':
        AddToken(TokenType.PLUS);
        break;
      case '-':
        AddToken(TokenType.MINUS);
        break;
      case '*':
        AddToken(TokenType.STAR);
        break;
      case ';':
        AddToken(TokenType.SEMICOLON);
        break;
      case '!':
        AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
        break;
      case '=':
        AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
        break;
      case '<':
        AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
        break;
      case '>':
        AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
        break;
      case '/':
        if (Match('/'))
          while (Peek() != '\n' && !IsAtEnd()) Advance();
        else
          AddToken(TokenType.SLASH);
        break;
      case ' ':
      case '\r':
      case '\t':
        break;
      case '\n':
        _line++;
        break;
      case '"':
        String();
        break;
      default:
        if (IsDigit(c))
          Number();
        else if (IsAlpha(c))
          Identifier();
        else
          Lox.Error(_line, "Unexpected character");
        break;
    }
  }

  private char Advance() => _code[_current++];
  private char Peek() => IsAtEnd() ? '\0' : _code[_current];
  private char PeekNext() => _current + 1 >= _code.Length ? '\0' : _code[_current + 1];

  private bool Match(char expected)
  {
    if (IsAtEnd()) return false;
    return _code[_current++] == expected;
  }

  private void AddToken(TokenType type) => AddToken(type, null);
  
  private void AddToken(TokenType type, object? literal)
  {
    _tokens.Add(new Token(type, _code.Substring(_start, _current - _start), literal, _line));
  }

  private bool IsAtEnd() => _current >= _code.Length;

  private void String()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\n') _line++;
      Advance();
    }

    if (IsAtEnd())
    {
      Lox.Error(_line, "Unterminated string.");
      return;
    }

    Advance();
    string value = _code.Substring(_start + 1, _current - _start - 2);
    AddToken(TokenType.STRING, value);
  }

  private bool IsDigit(char c) => c >= '0' && c <= '9';
  private bool IsAlpha(char c) => (c >= 'a' && c <= 'z')
                                  || (c >= 'A' && c <= 'Z')
                                  || c == '_';
  private bool IsAlphanumeric(char c) => IsAlpha(c) || IsDigit(c);
  
  private void Number()
  {
    while (IsDigit(Peek()))
      Advance();

    if (Peek() == '.' && IsDigit(PeekNext()))
    {
      Advance();
      while (IsDigit(Peek()))
        Advance();
    }
    
    AddToken(TokenType.NUMBER, double.Parse(_code.Substring(_start, _current - _start), CultureInfo.InvariantCulture));
  }

  private void Identifier()
  {
    while (IsAlphanumeric(Peek())) Advance();

    string text = _code.Substring(_start, _current - _start);
    TokenType? type = _keywords.ContainsKey(text) ? _keywords[text] : null;
    if (type == null) type = TokenType.IDENTIFIER;
    AddToken(type.Value);
  }
}
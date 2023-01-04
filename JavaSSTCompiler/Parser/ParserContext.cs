using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser
{
  public class ParserContext
  {
    private Queue<Token> _tokens;
    public ParserContext(IEnumerable<Token> tokens)
    {
      _tokens = new Queue<Token>(tokens);
    }

    public int TokensLeft => _tokens.Count;
    public Token CurrentToken => _tokens.Peek();
    public Token NextToken() => _tokens.Dequeue();

    public Token NextAndValidate(string errorMessage, params TokenType[] types)
    {
      var token = NextToken();
      if (types.Contains(token.Type) == false)
        throw new ParserException(token, errorMessage);

      return token;
    }

    public Token PeekAndValidate(string errorMessage, params TokenType[] types)
    {
      if (types.Contains(CurrentToken.Type) == false)
        throw new ParserException(CurrentToken, errorMessage);

      return CurrentToken;
    }

    public bool Is(params TokenType[] types) => types.Contains(CurrentToken.Type);

    public Token NextOptional(params TokenType[] types)
    {
      if (Is(types)) 
        return NextToken();
      
      return null;
    }
  }
}

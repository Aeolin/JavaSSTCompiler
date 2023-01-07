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
    /// <summary>
    /// Creates a new context for parsing a JavaSST class
    /// The context provides utility functions to check for
    /// token types and throw exceptions if they're not matching
    /// </summary>
    /// <param name="tokens">The token stream</param>
    public ParserContext(IEnumerable<Token> tokens)
    {
      _tokens = new Queue<Token>(tokens);
    }

    /// <summary>
    /// How many tokens are left in the queue
    /// </summary>
    public int TokensLeft => _tokens.Count;

    /// <summary>
    /// Peek at the current token
    /// </summary>
    public Token CurrentToken => _tokens.Peek();

    /// <summary>
    /// Get's the next token inside the queue
    /// </summary>
    /// <returns>The next token</returns>
    public Token NextToken() => _tokens.Dequeue();

    /// <summary>
    /// Gets the next token removing it from the queue and throws and exceptions if it is not of any expected type specified by <paramref name="types"/>
    /// </summary>
    /// <param name="errorMessage">The exception message in case the Token didnt match any of the expected type</param>
    /// <param name="types">The accepted Types for the token</param>
    /// <returns>The next Token if it matches any of the given Types</returns>
    /// <exception cref="ParserException">Throws exception if the next Token's type isn't expected</exception>
    public Token NextAndValidate(string errorMessage, params TokenType[] types)
    {
      var token = NextToken();
      if (types.Contains(token.Type) == false)
        throw new ParserException(token, errorMessage);

      return token;
    }
    /// <summary>
    /// Returns the Token at the front of the queue without removing it and throws and exceptions if it is not of any expected type specified by <paramref name="types"/>
    /// </summary>
    /// <param name="errorMessage">The exception message in case the Token didnt match any of the expected type</param>
    /// <param name="types">The accepted Types for the token</param>
    /// <returns>The next Token if it matches any of the given Types</returns>
    /// <exception cref="ParserException">Throws exception if the next Token's type isn't expected</exception>
    public Token PeekAndValidate(string errorMessage, params TokenType[] types)
    {
      if (types.Contains(CurrentToken.Type) == false)
        throw new ParserException(CurrentToken, errorMessage);

      return CurrentToken;
    }

    /// <summary>
    /// Checks if the type of the token in front of the queue is of any type given in <paramref name="types"/> 
    /// </summary>
    /// <param name="types">The types to test for</param>
    /// <returns>True if the next token's types is included in <paramref name="types"/></returns>
    public bool Is(params TokenType[] types) => types.Contains(CurrentToken.Type);

    /// <summary>
    /// Checks if the next token's type is included in <paramref name="types"/> and removes it from the queue if the type matches
    /// </summary>
    /// <param name="types">Allowed types for the next token</param>
    /// <returns>The next token if it's type is inside <paramref name="types"/> else null</returns>
    public Token NextOptional(params TokenType[] types)
    {
      if (Is(types)) 
        return NextToken();
      
      return null;
    }
  }
}

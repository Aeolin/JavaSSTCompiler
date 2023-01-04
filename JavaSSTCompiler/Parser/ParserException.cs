using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser
{
  public class ParserException : Exception
  {
    public Token token { get; init; }
    public ParserException(Token token, string message) : base(message)
    {
      this.token = token; 
    }

    public override string ToString()
    {
      return $"Error parsing token {token}\r\n" + base.ToString();
    }
  }
}

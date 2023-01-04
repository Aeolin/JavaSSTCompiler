using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Tokenizer
{
  public class Token
  {
    public TokenType Type { get; set; }
    public int Start { get; set; }
    public int Stop { get; set; }
    public string Value { get; set; }

    public Token(TokenType type, int start, int stop, string value)
    {
      Type=type;
      Start=start;
      Stop=stop;
      Value=value;
    }

    public override string ToString()
    {
      return $"{Type}@({Start}-{Stop}): {Value}";
    }
  }
}

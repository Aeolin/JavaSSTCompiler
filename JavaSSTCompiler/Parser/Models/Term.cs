using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class Term
  {
    public IFactor Factor { get; set; }
    public List<(TokenType Type, IFactor Factor)> Factors { get; init; } = new List<(TokenType Type, IFactor Factor)>();
  }
}

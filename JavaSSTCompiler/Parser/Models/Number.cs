using JavaSSTCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class Number : IFactor
  {
    public Number(ParserContext ctx)
    {
      var token = ctx.NextAndValidate("Expected a number", TokenType.Number);
      this.Value = int.Parse(token.Value);
    }

    public int Value { get; set; }
  }
}

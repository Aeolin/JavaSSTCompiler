using JavaSSTCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser
{
  public class AbstractIdentifiable
  {
    public string Identifier { get; init; }
    public AbstractIdentifiable(ParserContext ctx)
    {
      var token = ctx.NextAndValidate($"Expected identifier to contruct {this.GetType().Name}", TokenType.Identifier);
      Identifier = token.Value;
    }

    public AbstractIdentifiable(string identifier)
    {
      Identifier = identifier;
    }
  }
}

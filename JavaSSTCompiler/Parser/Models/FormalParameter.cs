using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class FormalParameter : AbstractIdentifiable
  {
    public FormalParameter(ParserContext ctx, Token type) : base(ctx)
    {
      this.Type = type;
    }
    
    public Token Type { get; set; }
  }
}

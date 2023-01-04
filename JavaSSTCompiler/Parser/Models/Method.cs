using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class Method : AbstractIdentifiable
  {
    public Method(ParserContext ctx, Token returnType) : base(ctx)
    {
      this.ReturnType = returnType;
    }

    public Token ReturnType { get; set; }
    public FormalParameter[] Parameters { get; set; }
    public MethodBody Body { get; set; }
  }
}

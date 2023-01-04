using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  internal class Identifier : AbstractIdentifiable, IFactor
  {
    public Identifier(ParserContext ctx) : base(ctx)
    {
    }

    public Identifier(string identifier) : base(identifier)
    {
    }
  }
}

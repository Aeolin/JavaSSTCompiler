using JavaSSTCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class Class : AbstractIdentifiable
  {
    public Class(ParserContext ctx) : base(ctx)
    {

    }

    public FinalField[] FinalFields { get; set; } //declarations 1
    public DynamicField[] DynamicFields { get; set; } //declarations 2
    public Method[] Methods { get; set; } //declarations 3
  }
}

﻿using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class FinalField : AbstractIdentifiable
  {
    public FinalField(ParserContext ctx) : base(ctx)
    {
    }

    public Token Type { get; set; }
    public Expression Expression { get; set; }
  }
}

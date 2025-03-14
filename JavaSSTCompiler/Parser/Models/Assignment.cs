﻿using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class Assignment : AbstractIdentifiable, IStatement
  {
    public Assignment(Token identifier) : base(identifier.Value)
    {
      
    }

    public Expression Expression { get; set; }
  }
}

﻿using JavaSSTCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class SimpleExpression 
  {
    public Term Term { get; set; }
    public List<(TokenType Operator, Term Term)> Terms { get; init; } = new List<(TokenType Operator, Term Term)>();
  }
}

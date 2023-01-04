﻿using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST
{
  public static class Extensions
  {
    public static TokenizerRuleAttribute GetRule(this TokenType value)
    {
      var type = typeof(TokenType);
      var field = type.GetField(value.ToString());
      return field.GetCustomAttribute<TokenizerRuleAttribute>();
    }
    
    public static byte[] ToBigEndian(this ushort value) => BitConverter.GetBytes(value).Reverse().ToArray();

  }
}

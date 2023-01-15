using JavaSST.Tokenizer;
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

    public static string VisualiserName(this TokenType value)
    {
      switch (value)
      {
        case TokenType.LessThan:
          return "<";
        case TokenType.GreaterThan:
          return ">";
        case TokenType.LessOrEqual:
          return "<=";
        case TokenType.GreaterOrEqual:
          return ">=";
        case TokenType.Equal:
          return "==";
        case TokenType.Plus:
          return "+";
        case TokenType.Minus:
          return "-";
        case TokenType.Multiply:
          return "*";
        case TokenType.Divide:
          return "/";
        default:
          return value.ToString();
      }
    }

    public static byte[] ToBigEndian(this ushort value) => BitConverter.GetBytes(value).Reverse().ToArray();

  }
}

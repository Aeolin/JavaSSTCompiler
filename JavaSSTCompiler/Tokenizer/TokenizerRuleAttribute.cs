using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Tokenizer
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public class TokenizerRuleAttribute : Attribute
  {
    public string Regex { get; set; }
    public TokenizerRuleAttribute(string regex)
    {
      Regex = regex;
    }
  }
}

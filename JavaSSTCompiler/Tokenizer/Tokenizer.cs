using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Tokenizer
{
  public class Tokenizer
  {

    private static readonly Dictionary<TokenType, Regex> RULES = new Dictionary<TokenType, Regex>();
    public bool SkipWhitespaces { get; set; }

    static Tokenizer()
    {
      RULES.Clear();
      foreach (var token in Enum.GetValues<TokenType>())
        RULES[token] = new Regex("^"+ token.GetRule().Regex, RegexOptions.Compiled);
    }

    public Tokenizer(bool skipWhitespaces = true)
    {
      SkipWhitespaces = skipWhitespaces;
    }

    public IEnumerable<Token> Tokenize(Stream input)
    {
      using var reader = new StreamReader(input);
      var source = reader.ReadToEnd();
      int countRead = 0;
      while (source.Length > 0)
      {
        var ruleFound = false;
        foreach (var rule in RULES)
        {
          var match = rule.Value.Match(source);
          if (match.Success)
          {
            ruleFound = true;
            source = source.Substring(match.Length);
            if (rule.Key != TokenType.Whitespace || SkipWhitespaces == false)
              yield return new Token(rule.Key, countRead, countRead + match.Length, match.Value);
            countRead += match.Length;
            break;
          }
        }
        if (ruleFound == false)
          throw new Exception($"No matching token found for remaining input {source}");

      }
    }
  }
}
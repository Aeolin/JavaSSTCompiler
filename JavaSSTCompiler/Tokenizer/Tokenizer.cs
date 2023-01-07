using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaSST.Tokenizer
{
  public class Tokenizer
  {

    private static readonly Dictionary<TokenType, Regex> RULES = new Dictionary<TokenType, Regex>();
    public bool SkipWhitespaces { get; set; }

    /// <summary>
    /// Constructs a lookup table of TokenTypes and their corresponding Regexes.
    /// Informations are pulled from the TokenizerRule attributes inside the TokenType enum
    /// </summary>
    static Tokenizer()
    {
      RULES.Clear();
      foreach (var token in Enum.GetValues<TokenType>())
        RULES[token] = new Regex("^"+ token.GetRule().Regex, RegexOptions.Compiled);
    }

    /// <summary>
    /// Constrcuts a new Tokenizer
    /// </summary>
    /// <param name="skipWhitespaces">wheter the TokenType.Whitespace should be emitted or not</param>
    public Tokenizer(bool skipWhitespaces = true)
    {
      SkipWhitespaces = skipWhitespaces;
    }

    /// <summary>
    /// Tokenizes the given input stream into the tokens specified in the TokenType enum
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IEnumerable<Token> Tokenize(Stream input)
    {
      using var reader = new StreamReader(input);
      var source = reader.ReadToEnd();
      int countRead = 0;
      while (source.Length > 0)
      {
        var ruleFound = false;
        foreach (var rule in RULES) // iterate over all available rules until a matching one is found
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
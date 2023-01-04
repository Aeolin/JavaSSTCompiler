using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Tokenizer
{
  public enum TokenType
  {
    [TokenizerRule(@"\/\*[\S\s]*?\*\/")]
    Comment,
    [TokenizerRule("if")]
    If,
    [TokenizerRule("else")]
    Else,
    [TokenizerRule("while")]
    While,
    [TokenizerRule("return")]
    Return,
    [TokenizerRule(",")]
    Comma,
    [TokenizerRule("void")]
    Void,
    [TokenizerRule("public")]
    Public,
    [TokenizerRule("final")]
    Final,
    [TokenizerRule("class")]
    Class,
    [TokenizerRule("int")]
    Type,
    [TokenizerRule("==")]
    Equal,
    [TokenizerRule("<=")]
    LessOrEqual,
    [TokenizerRule("<")]
    LessThan,
    [TokenizerRule(">=")]
    GreaterOrEqual,
    [TokenizerRule(">")]
    GreaterThan,
    [TokenizerRule("\\(")]
    LParen,
    [TokenizerRule("\\)")]
    RParen,
    [TokenizerRule("{")]
    LCurly,
    [TokenizerRule("}")]
    RCurly,
    [TokenizerRule(";")]
    Semicolon,
    [TokenizerRule("=")]
    Assign,
    [TokenizerRule("\\+")]
    Plus,
    [TokenizerRule("-")]
    Minus,
    [TokenizerRule("\\/")]
    Divide,
    [TokenizerRule("\\*")]
    Multiply,
    [TokenizerRule("[0-9]+")]
    Number,
    [TokenizerRule("[a-zA-Z]([0-9a-zA-Z])*")]
    Identifier,
    [TokenizerRule("[ \t\n\r]")]
    Whitespace
  }
}

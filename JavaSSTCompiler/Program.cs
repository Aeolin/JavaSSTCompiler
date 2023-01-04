using Antlr4.Runtime;
using JavaSST;
using JavaSST.Parser;
using JavaSST.Tokenizer;
using System.CommandLine;
using Parser = JavaSST.Parser.Parser;

namespace JavaSST
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var root = new RootCommand("JavaSST Parser");
      var inputFileOption = new Option<FileInfo>(new[] { "-i", "--input" }, "The input file to parse.");
      var outputFileOption = new Option<FileInfo>(new[] { "-o", "--output" }, () => null, "The output file to write to.");
      root.AddOption(inputFileOption);
      root.AddOption(outputFileOption);
      root.SetHandler(Compile, inputFileOption, outputFileOption);
      root.Invoke(args);
    }

    public static void Compile(FileInfo input, FileInfo output)
    {
      if (output == null)
        output = new FileInfo(Path.ChangeExtension(input.FullName, ".class"));

      var tokenizer = new Tokenizer();
      var tokens = tokenizer.Tokenize(input.OpenRead()).ToArray();
      var parser = new Parser();
      var ast = parser.Parse(tokens);

      //var lexer = new java_sstLexer(input);
      //var tokens = new CommonTokenStream(lexer);
      //var parser = new java_sstParser(tokens);
      //var tree = parser.class_();
      //var visitor = new JavaSSTVisitor();
      //var result = visitor.Visit(tree);

      //using (var writer = new BigEndianBinaryWriter(output.Create()))
      //{
        
      //}
    }
  }
}
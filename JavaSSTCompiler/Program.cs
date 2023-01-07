using JavaSST;
using JavaSST.Compiler;
using JavaSST.Parser;
using JavaSST.Tokenizer;
using JavaSSTCompiler.Compiler.Builder.ConstantPool;
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

      var tokenizer = new Tokenizer.Tokenizer();
      var tokens = tokenizer.Tokenize(input.OpenRead()).ToArray();
      var parser = new Parser.Parser();
      var ast = parser.Parse(tokens);
      var compiler = new Compiler.Compiler();
      var bytecode = compiler.Compile(ast, input.Name);
      File.WriteAllBytes(output.FullName, bytecode);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool
{
  public class FieldDescriptorBuilder : DescriptorBuilder
  {
    private string _type;

    public FieldDescriptorBuilder()
    {

    }

    public FieldDescriptorBuilder WithType(string type)
    {
      _type = TranslateType(type);
      return this;
    }

    public override string Build()
    {
      if (_type == null)
        throw new InvalidOperationException("Type is not set");
      
      return _type;
    }
  }

  public class MethodDescriptorBuilder : DescriptorBuilder
  {
    private List<string> _parameters = new List<string>();
    private string _returnType;

    public MethodDescriptorBuilder()
    {

    }

    public MethodDescriptorBuilder WithParameter(params string[] parameters)
    {
      foreach (var parameter in parameters)
        _parameters.Add(TranslateType(parameter));

      return this;
    }

    public MethodDescriptorBuilder WithParameters(IEnumerable<string> parameters)
    {
      if (parameters == null)
        return this;

      foreach (var parameter in parameters)
        _parameters.Add(TranslateType(parameter));

      return this;
    }

    public MethodDescriptorBuilder WithReturnType(string type)
    {
      _returnType = TranslateType(type);
      return this;
    }

    public override string Build()
    {
      if(_returnType == null)
        throw new InvalidOperationException("Return type is not set");

      return $"({string.Join("", _parameters)}){_returnType}";
    }
  }

  public abstract class DescriptorBuilder
  {
    private static readonly Dictionary<string, string> TYPE_TRANSLATIONS = new Dictionary<string, string>
    {
      {"byte", "B"},
      {"char", "C"},
      {"double", "D"},
      {"float", "F"},
      {"int", "I"},
      {"long", "J"},
      {"short", "S"},
      {"boolean", "Z"},
      {"void", "V"}
    };

    public abstract string Build();

    protected string TranslateType(string type)
    {
      int arrayOrder = 0;
      while ((type.LastIndexOf("[]")) != -1)
      {
        type = type.Remove(type.Length - 2);
        arrayOrder++;
      }

      if (TYPE_TRANSLATIONS.TryGetValue(type, out var res) == false)
        res = $"L{type};";

      if (arrayOrder > 0)
        res = new string('[', arrayOrder) + res;

      return res;
    }
  }
}

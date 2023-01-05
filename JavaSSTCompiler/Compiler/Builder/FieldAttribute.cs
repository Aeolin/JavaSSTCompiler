using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class FieldAttribute : Attribute
  {
    public int Index { get; set; }
    public Type Type { get; set; }
    public Type PrefixLenType { get; set; }

    public FieldAttribute(int index, Type type = null)
    {
      Index = index;
      Type = type;
    }
  }
}

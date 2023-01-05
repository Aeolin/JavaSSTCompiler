using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder
{
  public abstract class AbstractSerializable
  {

    private static readonly Dictionary<Type, MethodInfo> WRITE = new Dictionary<Type, MethodInfo>();

    static AbstractSerializable()
    {
      var methods = typeof(BinaryWriter)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(x => x.Name == "Write" && x.GetParameters().Length == 1);

      foreach (var method in methods)
        WRITE[method.GetParameters().First().ParameterType] = method;
    }

    public virtual byte[] ToBytes()
    {
      using var mem = new MemoryStream();
      using var writer = new ConstantPoolBinaryWriter(mem);
      var properties = this.GetType()
        .GetProperties()
        .Select(x => (prop: x, attr: x.GetCustomAttribute<FieldAttribute>()))
        .Where(x => x.attr != null)
        .OrderBy(x => x.attr.Index)
        .ToArray();

      foreach (var property in properties)
      {
        if (property.prop.PropertyType.IsArray)
        {
          var elementType = property.attr.Type ?? property.prop.PropertyType.GetElementType();
          var value = (Array)property.prop.GetValue(this);
          if (property.attr.PrefixLenType != null)
            WRITE[property.attr.PrefixLenType].Invoke(writer, new[] { Convert.ChangeType(value.LongLength, property.attr.PrefixLenType) });

          if (elementType == typeof(byte))
          {
            writer.Write((byte[])value);
          }
          else
          {
            foreach (var item in value)
              WRITE[elementType].Invoke(writer, new[] { Convert.ChangeType(item, elementType) });
          }

        }
        else
        {
          var type = property.attr.Type ?? property.prop.PropertyType;
          WRITE[type].Invoke(writer, new[] { Convert.ChangeType(property.prop.GetValue(this), type) });
        }

      }

      return mem.ToArray();
    }
  }
}

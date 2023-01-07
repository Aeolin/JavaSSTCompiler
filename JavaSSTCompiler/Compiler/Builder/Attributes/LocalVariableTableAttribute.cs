using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Attributes
{
    public class LocalVariableTableAttribute : AbstractAttribute
  {

    [Field(1, Type = typeof(AbstractSerializable), PrefixLenType = typeof(ushort))]
    public LocalVariableTableEntry[] LocalVariableTableEntries { get; init; }

    public LocalVariableTableAttribute(ConstantPool.ConstantPool pool, LocalVariableTableEntry[] localVariables) : base(pool.Utf8Info("LocalVariableTable"))
    {
      LocalVariableTableEntries = localVariables;
    }

    public static LocalVariableTableAttributeBuilder CreateBuilder(ConstantPool.ConstantPool pool) => new LocalVariableTableAttributeBuilder(pool);

    public class LocalVariableTableAttributeBuilder
    {
      private ConstantPool.ConstantPool _constantPool; 
      private List<LocalVariableTableEntry> _entries = new List<LocalVariableTableEntry>();

      public LocalVariableTableAttributeBuilder(ConstantPool.ConstantPool pool)
      {
        _constantPool = pool;
      }

      public LocalVariableTableAttributeBuilder WithVariable(string name, string type, ushort index, ushort start, ushort length)
      {
        _entries.Add(new LocalVariableTableEntry(_constantPool, name, type, index, start, length));
        return this;
      }

      public LocalVariableTableAttribute Build()
      {
        return new LocalVariableTableAttribute(_constantPool, _entries.ToArray());
      }
    }
  }
}

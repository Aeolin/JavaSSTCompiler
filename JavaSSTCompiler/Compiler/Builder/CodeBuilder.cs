using JavaSST.Compiler.Models;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder
{
  public class CodeBuilder
  {
    private ConstantPool.ConstantPool _constantPool;
    private ClassInfo _classInfo;
    private Method _method;
    private MemoryStream _code;
    private List<(string name, int index)> _localVars = new List<(string, int)>();
    
    public int MaxStack { get; private set; }
    public int MaxLocals { get; private set; }

    public CodeBuilder(ConstantPool.ConstantPool constantPool, Method method, ClassInfo classInfo)
    {
      _constantPool = constantPool;
      _code = new MemoryStream();
      _method = method;
      _classInfo = classInfo;
      MaxLocals = 1 + method.Parameters.Length;
    }

    public CodeBuilder Aload(byte index)
    {
      if (index < 4)
      {
        _code.WriteByte((byte)(index + 0x2a));
      }
      else
      {
        _code.WriteByte(0x19);
        _code.WriteByte(index);
      }
      MaxStack++;
      return this;
    }

    public CodeBuilder DefineLocal(string name)
    {
      var index = MaxLocals++;
      _localVars.Add((name, index));
      return this;
    }

    public CodeBuilder InvokeVirtual(RefInfo method)
    {
      _code.WriteByte(0xb6);
      _code.Write(BitConverter.GetBytes(method.Index).Reverse().ToArray());
      MaxStack--;
      return this;
    }

    public CodeBuilder InvokeSpecial(RefInfo method)
    {
      _code.WriteByte(0xb7);
      _code.Write(BitConverter.GetBytes(method.Index).Reverse().ToArray());
      MaxStack--;
      return this;
    }

    public CodeBuilder StoreIntVariable(string variable, bool loadSelfRef = false)
    {
      var index = Array.FindIndex(_method.Parameters, x => x.Name == variable);
      if (index == -1)
        index = _localVars.FirstOrDefault(x => x.name == variable, (null, -1)).index;

      if (index == -1)
      {
        if (loadSelfRef)
        {
          Aload(0);
          Swap();
        }
        var fieldRef = _constantPool.FieldRefInfo(_classInfo, variable, "int");
        _code.WriteByte(0xb5);
        _code.Write(BitConverter.GetBytes(fieldRef.Index).Reverse().ToArray());
      }
      else
      {
        if (index < 4)
        {
          _code.WriteByte((byte)(0x59 + index));
        }
        else
        {
          _code.WriteByte(0x54);
          _code.WriteByte((byte)index);
        }
      }
      MaxStack++;
      return this;
    }

    public CodeBuilder Swap()
    {
      _code.WriteByte(0x5f);
      return this;
    }
    
    public CodeBuilder LoadIntVariable(string variable, bool loadSelfRef = false)
    {
      var index = Array.FindIndex(_method.Parameters, x => x.Name == variable);
      if(index == -1)
        index = _localVars.FirstOrDefault(x => x.name == variable, (null, -1)).index;

      if (index == -1)
      {
        if (loadSelfRef)
        {
          Aload(0);
          Swap();
        }
        var fieldRef = _constantPool.FieldRefInfo(_classInfo, variable, "int");
        _code.WriteByte(0xb4);
        _code.Write(BitConverter.GetBytes(fieldRef.Index).Reverse().ToArray());
      }
      else
      {
        if (index < 4)
        { 
          _code.WriteByte((byte)(0x1a + index));
        }
        else
        {
          _code.WriteByte(0x21);
          _code.WriteByte((byte)index);
        }
      }
      MaxStack++;
      return this;
    }
    
    public CodeBuilder Return()
    {
      _code.WriteByte(0xb1);
      return this;
    }
    
    public CodeBuilder ConstInt(int value)
    {
      if (value >= -1 && value <= 5)
      {
        _code.WriteByte((byte)(0x03 + value));
      }
      else if (value >= -128 && value <= 127)
      {
        _code.WriteByte(0x10);
        _code.WriteByte((byte)value);
      }
      else if (value >= -32768 && value <= 32767)
      {
        _code.WriteByte(0x11);
        _code.Write(BitConverter.GetBytes((short)value).Reverse().ToArray());
      }
      else
      {
        _code.WriteByte(0x12);
        _code.Write(BitConverter.GetBytes(value).Reverse().ToArray());
      }
      MaxStack++;
      return this;
    }

    public byte[] Build() => _code.ToArray();
  }
}

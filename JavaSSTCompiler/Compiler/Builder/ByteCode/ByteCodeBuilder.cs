using JavaSST.Compiler.Models;
using JavaSSTCompiler.Compiler.Builder.Attributes;
using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ByteCode
{
  public class ByteCodeBuilder
  {
    public ClassBuilder ClassBuilder { get; init; }
    private Method _method;
    private MemoryStream _code;
    private BigEndianBinaryWriter _writer;
    private Dictionary<string, int> _localVariables = new Dictionary<string, int>();

    public ushort MaxStack { get; private set; }
    private ushort _currentStack = 0;

    public ushort CurrentStack
    {
      get => _currentStack;
      set
      {
        MaxStack = Math.Max(MaxStack, value);
        _currentStack = value;
      }
    }
    
    public ushort MaxLocals { get; private set; }
    public ushort CurrentAddress => (ushort)_writer.BaseStream.Position;


    
    public ByteCodeBuilder(ClassBuilder builder, Method method)
    {
      _code = new MemoryStream();
      _writer = new BigEndianBinaryWriter(_code);
      _method = method;
      ClassBuilder = builder;
      MaxLocals = 1;
      foreach (var parameter in method.Parameters)
        _localVariables[parameter.Name] = MaxLocals++;
    }

    public ByteCodeBuilder DefineLocal(string name)
    {
      if (_localVariables.ContainsKey(name))
        throw new ArgumentException($"local variable with {name} already exists for {_method.Name}");

      _localVariables[name] = MaxLocals++;
      return this;
    }

    public ByteCodeBuilder Aload(byte index)
    {
      if (index < 4)
      {
        _writer.WriteByte((byte)(index + 0x2a));
      }
      else
      {
        _writer.WriteByte(0x19);
        _writer.WriteByte(index);
      }
      CurrentStack++;
      return this;
    }

    public ByteCodeBuilder IAdd()
    {
      _writer.WriteByte(0x60);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder ISub()
    {
      _writer.WriteByte(0x64);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder IMul()
    {
      _writer.WriteByte(0x68);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder IDiv()
    {
      _writer.WriteByte(0x6c);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder IfICmp(CompareType cmp, out ByteCodeMarker marker)
    {
      _writer.WriteByte(0x9f + (byte)cmp);
      marker = new ByteCodeMarker(_writer, 2);
      _writer.Write((ushort)0);
      CurrentStack -= 2;
      return this;
    }

    /// <summary>
    /// Inserts an IfICmp instruction
    /// Uses absolute address to calculate relative jump position
    /// </summary>
    /// <param name="cmp">The comparison type</param>
    /// <param name="absoluteAddress">The absolute index in the method body the if should jump to on successfull comparison</param>
    /// <returns></returns>
    public ByteCodeBuilder IfICmp(CompareType cmp, ushort absoluteAddress)
    {
      var offset = CurrentAddress;
      short relativeAddress = (short)(absoluteAddress - offset);
      _writer.WriteByte((byte)(0x9f + cmp));
      _writer.Write(relativeAddress);
      CurrentStack -= 2;
      return this;
    }

    public ByteCodeBuilder Goto(out ByteCodeMarker marker)
    {
      _writer.WriteByte(0xa7);
      marker = new ByteCodeMarker(_writer, 2);
      _writer.Write((ushort)0x00);
      return this;
    }

    public ByteCodeBuilder Goto(ushort address)
    {
      _writer.WriteByte(0xa7);
      _writer.Write(address);
      return this;
    }

    public ByteCodeBuilder InvokeVirtual(RefInfo method)
    {
      if (method.Tag != ConstantPoolTag.MethodRef)
        throw new ArgumentException("expected reference to method");

      _writer.WriteByte(0xb6);
      _writer.Write(method.Index);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder InvokeSpecial(RefInfo method)
    {
      _writer.WriteByte(0xb7);
      _writer.Write(method.Index);
      CurrentStack--;
      return this;
    }

    public ByteCodeBuilder StoreIntVariable(string variable, bool loadSelfRef = false)
    {
      if (_localVariables.TryGetValue(variable, out var index))
      {
        if (index < 4)
        {
          _writer.WriteByte((byte)(0x3b + index));
        }
        else
        {
          _writer.WriteByte(0x36);
          _writer.WriteByte(index);
        }
      }
      else
      {
        if (loadSelfRef)
        {
          Aload(0);
          Swap();
        }
        var fieldRef = ClassBuilder.ConstantPool.FieldRefInfo(ClassBuilder.ThisClassInfo, variable, "int");
        _writer.WriteByte(0xb5);
        _writer.Write(fieldRef.Index);
      }

      CurrentStack++;
      return this;
    }

    public ByteCodeBuilder Swap()
    {
      _writer.WriteByte(0x5f);
      return this;
    }

    public ByteCodeBuilder LoadIntVariable(string variable)
    {
      if (_localVariables.TryGetValue(variable, out var index))
      {
        if (index < 4)
        {
          _writer.WriteByte((byte)(0x1a + index));
        }
        else
        {
          _writer.WriteByte(0x21);
          _writer.WriteByte((byte)index);
        }
      }
      else
      {
        Aload(0);
        var fieldRef = ClassBuilder.ConstantPool.FieldRefInfo(ClassBuilder.ThisClassInfo, variable, "int");
        _writer.WriteByte(0xb4);
        _writer.Write(fieldRef.Index);
      }

      CurrentStack++;
      return this;
    }

    public ByteCodeBuilder Return()
    {
      _writer.WriteByte(0xb1);
      return this;
    }

    public ByteCodeBuilder IReturn()
    {
      _writer.WriteByte(0xac);
      return this;
    }

    public ByteCodeBuilder ConstInt(int value)
    {
      if (value >= -1 && value <= 5)
      {
        _writer.WriteByte((byte)(0x03 + value));
      }
      else if (value >= -128 && value <= 127)
      {
        _writer.WriteByte(0x10);
        _writer.WriteByte((byte)value);
      }
      else if (value >= -32768 && value <= 32767)
      {
        _writer.WriteByte(0x11);
        _writer.Write((short)value);
      }
      else
      {
        _writer.WriteByte(0x12);
        _writer.Write(value);
      }
      CurrentStack++;
      return this;
    }

    public IEnumerable<AbstractAttribute> Build()
    {
      var codeAttribute = new CodeAttribute(ClassBuilder.ConstantPool);
      codeAttribute.MaxStack = MaxStack;
      codeAttribute.MaxLocals = MaxLocals;
      codeAttribute.Code = _code.ToArray();

      var tableBuilder = LocalVariableTableAttribute.CreateBuilder(ClassBuilder.ConstantPool);
      foreach (var local in _localVariables)
        tableBuilder.WithVariable(local.Key, "int", (ushort)local.Value, 0, (ushort)_code.Length);

      codeAttribute.Attributes = new AbstractAttribute[] { tableBuilder.Build() };
      yield return codeAttribute;
    }    
  }
}

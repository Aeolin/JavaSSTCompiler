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
    public ClassBuilder ClassBuilder { get; init; }
    private Method _method;
    private MemoryStream _code;
    private List<(string name, int index)> _localVars = new List<(string, int)>();

    public ushort MaxStack { get; private set; }
    public ushort MaxLocals { get; private set; }
    //public ushort CurrentAddress => (ushort) _code.Position-1;
    public ushort NextAddress => (ushort)(_code.Position);

    public CodeBuilder(ClassBuilder builder, Method method)
    {
      _code = new MemoryStream();
      _method = method;
      ClassBuilder = builder;
      MaxLocals = (ushort)(1 + method.Parameters.Length);
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

    public CodeBuilder IAdd()
    {
      _code.WriteByte(0x60);
      MaxStack--;
      return this;
    }

    public CodeBuilder ISub()
    {
      _code.WriteByte(0x64);
      MaxStack--;
      return this;
    }

    public CodeBuilder IMul()
    {
      _code.WriteByte(0x68);
      MaxStack--;
      return this;
    }

    public CodeBuilder IDiv()
    {
      _code.WriteByte(0x6c);
      MaxStack--;
      return this;
    }

    public enum CompareType
    {
      Equal = 0,
      NotEqual = 1,
      LessThan = 2,
      GreaterThanOrEqual = 3,
      GreaterThan = 4,
      LessThanOrEqual = 5,
    }

    public class Marker 
    {
      public long MarkerPosition { get; init; }
      private MemoryStream _stream;
      private int _length;

      public Marker(MemoryStream stream, int len)
      {
        _stream = stream;
        MarkerPosition = stream.Position;
        _length = len;
      }

      public void WriteRelative(ushort address)
      {
        var instrPos = MarkerPosition-1;
        short offset = (short)(address-instrPos);
        Write(BitConverter.GetBytes(offset).Reverse().ToArray());
      }
      
      public void Write(ushort data) => Write(BitConverter.GetBytes(data).Reverse().ToArray());

      public void Write(byte[] data)
      {
        if (data.Length != _length)
          throw new ArgumentException($"Invalid length, expected {_length}, got {data.Length}");

        var temp = _stream.Position;
        _stream.Seek(MarkerPosition, SeekOrigin.Begin);
        _stream.Write(data);
        _stream.Seek(temp, SeekOrigin.Begin);
      }
    }
    

    public CodeBuilder IfICmp(CompareType cmp, out Marker marker)
    {
      _code.WriteByte((byte)(0x9f + cmp));
      marker = new Marker(_code, 2);
      _code.WriteByte(0x00);
      _code.WriteByte(0x00);
      MaxStack-=2;
      return this;
    }

    public CodeBuilder IfICmp(CompareType cmp, ushort address)
    {
      var offset = NextAddress;
      short relativeAddress = (short)(address-offset);
      _code.WriteByte((byte)(0x9f + cmp));
      _code.Write(BitConverter.GetBytes(relativeAddress).Reverse().ToArray());
      MaxStack-=2;
      return this;
    }

    public CodeBuilder Goto(out Marker marker)
    {
      _code.WriteByte(0xa7);
      marker = new Marker(_code, 2);
      _code.WriteByte(0x00);
      _code.WriteByte(0x00);
      return this;
    }

    public CodeBuilder Goto(ushort address)
    {
      _code.WriteByte(0xa7);
      _code.Write(BitConverter.GetBytes(address).Reverse().ToArray());
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
      else
        index++; // increment index because local var0 = this

      if (index == -1)
      {
        if (loadSelfRef)
        {
          Aload(0);
          Swap();
        }
        var fieldRef = ClassBuilder.ConstantPool.FieldRefInfo(ClassBuilder.ThisClassInfo, variable, "int");
        _code.WriteByte(0xb5);
        _code.Write(BitConverter.GetBytes(fieldRef.Index).Reverse().ToArray());
      }
      else
      {
        if (index < 4)
        {
          _code.WriteByte((byte)(0x3b + index));
        }
        else
        {
          _code.WriteByte(0x36);
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

    public CodeBuilder LoadIntVariable(string variable)
    {
      var index = Array.FindIndex(_method.Parameters, x => x.Name == variable);
      if (index == -1)
        index = _localVars.FirstOrDefault(x => x.name == variable, (null, -1)).index;
      else
        index++; // increment index because local var0 = this
      
      if (index == -1)
      {

        Aload(0);
        var fieldRef = ClassBuilder.ConstantPool.FieldRefInfo(ClassBuilder.ThisClassInfo, variable, "int");
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

    public CodeBuilder IReturn()
    {
      _code.WriteByte(0xac);
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

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool
{
  public class ConstantPoolBinaryWriter : BinaryWriter
  {
    public ConstantPoolBinaryWriter(Stream stream) : base(stream)
    {
    }

    public ConstantPoolBinaryWriter(Stream stream, Encoding encoding) : base(stream, encoding)
    {
    }

    public ConstantPoolBinaryWriter(Stream stream, Encoding encoding, bool leaveOpen) : base(stream, encoding, leaveOpen)
    {
    }

    public override void Write(short value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(int value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(long value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(ushort value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(uint value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(ulong value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(float value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(double value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(char value)
    {
      var bytes = BitConverter.GetBytes(value);
      Array.Reverse(bytes);
      base.Write(bytes);
    }

    public override void Write(char[] chars)
    {
      foreach (var c in chars)
        Write(c);
    }

    public override void Write(char[] chars, int index, int count)
    {
      for (int i = index; i < index + count; i++)
        Write(chars[i]);
    }

    public void Write(AbstractSerializable serializable) => Write(serializable.ToBytes());

    public override void Write(string value)
    {
      var pos = this.BaseStream.Position;
      Write((ushort)0);
      ushort count = 0;
      for (int i = 0; i < value.Length; i++)
      {
        var c = char.ConvertToUtf32(value, i);
        if (c > 0xFFFF)
          i++;

        count++;
        if (c >= 0x0001 && c <= 0x007F)
        { 
          Write((byte)c);
        }
        else if (c == 0x0000 || (c >= 0x0080 && c <= 0x07FF))
        {
          Write((byte)(0xC0 | (0x1F & (c >> 6))));
          Write((byte)(0x80 | (0x3F & c)));
        }
        else if (c >= 0x0800 && c <= 0xFFFF)
        {
          Write((byte)(0xE0 | (0x0F & (c >> 12))));
          Write((byte)(0x80 | (0x3F & (c >> 6))));
          Write((byte)(0x80 | (0x3F & c)));
        }
        else if (c > 0xFFFF)
        {
          Write((byte)0xED);
          Write((byte)value[i] >> 8);
          Write((byte)value[i]);
          Write((byte)0xED);
          Write((byte)value[i+1] >> 8);
          Write((byte)value[i+1]);
        } 
      }
      var newPos = this.BaseStream.Position;
      this.BaseStream.Seek(pos, SeekOrigin.Begin);
      Write(count);
      this.BaseStream.Seek(newPos, SeekOrigin.Begin);
    }
  }
}

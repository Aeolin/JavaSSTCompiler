using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler
{
  public class BigEndianBinaryWriter : BinaryWriter
  {
    public BigEndianBinaryWriter(Stream stream) : base(stream)
    {
    }

    public BigEndianBinaryWriter(Stream stream, Encoding encoding) : base(stream, encoding)
    {
    }

    public BigEndianBinaryWriter(Stream stream, Encoding encoding, bool leaveOpen) : base(stream, encoding, leaveOpen)
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

    public override void Write(string value)
    {
      foreach (var c in value)
        Write(c);
    }
  }
}

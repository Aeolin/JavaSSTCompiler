using JavaSSTCompiler.Compiler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ByteCode
{
  public class ByteCodeMarker
  {
    public long MarkerPosition { get; init; }
    private BigEndianBinaryWriter _writer;
    private int _length;

    public ByteCodeMarker(BigEndianBinaryWriter writer, int len)
    {
      _writer = writer;
      MarkerPosition = _writer.BaseStream.Position;
      _length = len;
    }

    public void WriteRelative(ushort address)
    {
      if (_length != 2)
        throw new ArgumentException($"Invalid length, expected {_length}, got 2");

      var temp = _writer.BaseStream.Position;
      _writer.BaseStream.Seek(MarkerPosition, SeekOrigin.Begin);
      var instrPos = MarkerPosition - 1;
      short offset = (short)(address - instrPos);
      _writer.Write(offset);
      _writer.BaseStream.Seek(temp, SeekOrigin.Begin);
    }
  }
}

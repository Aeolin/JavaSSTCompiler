using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ByteCode
{
  public class LocalVariable
  {
    public ushort FirstUsage { get; set; }
    public ushort LastUsage { get; set; }
    public ushort Index { get; set; }
    public bool IsInitialized { get; set; }
    public string Name { get; set; }

    public LocalVariable(string name, ushort index, bool isInitialized, ushort firstUsage = ushort.MaxValue)
    {
      Name = name;
      Index = index;
      IsInitialized = isInitialized;
      FirstUsage = firstUsage;
    }

    public void UpdateUsage(ushort usage)
    {
      FirstUsage = Math.Min(FirstUsage, usage);
      LastUsage = Math.Max(LastUsage, usage);
    }

    public void LimitFirstUsage(ushort currentAddress)
    {
      if (FirstUsage > currentAddress)
        FirstUsage = currentAddress;
      if (LastUsage < FirstUsage)
        LastUsage = FirstUsage;
    }
  }
}

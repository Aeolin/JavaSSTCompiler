using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  public class ProcedureCall : AbstractIdentifiable, IFactor, IStatement
  {
    public ProcedureCall(string identifier) : base(identifier)
    {
    }

    

    public string MethodSignature { get; set; }
    public List<Expression> Arguments { get; init; } = new List<Expression>();
  }
}

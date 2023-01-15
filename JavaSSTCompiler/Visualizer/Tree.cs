using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Visualizer
{
  public class Tree
  {
    public Node Root { get; init; }
    public Dictionary<int, List<Node>> NodeLevels { get; init; } = new Dictionary<int, List<Node>>();
    public Tree(Node root)
    {
      Root = root;
      generateNodeLevels(root);
    }

    public void ForEach(Action<Node> action)
    {
      foreach(var node in NodeLevels.Values.SelectMany(x => x))
        action(node);
    }

    public IEnumerable<PointF> GetRect()
    {
      var nodes = NodeLevels.Values.SelectMany(x => x);
      var minX = nodes.Min(x => x.X);
      var maxX = nodes.Max(x => x.X);
      var minY = nodes.Min(x => x.Y);
      var maxY = nodes.Max(x => x.Y);

      yield return new PointF(minX, minY);
      yield return new PointF(maxX, minY);
      yield return new PointF(maxX, maxY);
      yield return new PointF(minX, maxY);
    }
    
    public Size GetSize()
    {
      var nodes = NodeLevels.Values.SelectMany(x => x);
      var maxX = nodes.Max(x => x.X + x.Width);
      var maxY = nodes.Max(x => x.Y + x.Height);
      return new Size((int)maxX, (int)maxY);
    }


    protected void generateNodeLevels(Node node)
    {
      if(NodeLevels.TryGetValue(node.Level, out var levels) == false)
      {
        levels =new List<Node>();
        NodeLevels[node.Level] = levels;
      }

      levels.Add(node);
      node.Childrens.ForEach(generateNodeLevels);
    } 


  }
}

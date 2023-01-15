using SixLabors.Fonts;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Visualizer
{
  public class Node
  {
    public string Text { get; init; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public FontRectangle TextMeasure { get; set; }

    public int Level { get; init; }
    public List<Node> Childrens { get; init; } = new List<Node>();

    public void SetBounds(float x, float y, float width, float height)
    {
      this.X = x;
      this.Y = y;
      this.Width = width;
      this.Height = height;
    }

    public IEnumerable<PointF> GetRect()
    {
      yield return new PointF(X, Y);
      yield return new PointF(X + Width, Y);
      yield return new PointF(X + Width, Y + Height);
      yield return new PointF(Width, Y + Height);
    }

    public PointF TextLocation()
    {
      var middle = Middle();
      middle.X -= (TextMeasure.Width/2);
      middle.Y -= (TextMeasure.Height/2);
      return middle;
    }

    public PointF Middle() => new PointF(X + (Width/2), Y+(Height/2));
    public PointF CenterBottom() => new PointF(X + (Width/2), Y + Height);
    public PointF CenterTop() => new PointF(X+(Width/2), Y);

    public Node(string text, Node parent = null)
    {
      parent?.Childrens?.Add(this);
      Text=text;
      Level= 1+((parent?.Level) ?? -1);
    }
  }
}

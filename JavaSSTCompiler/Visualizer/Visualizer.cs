using JavaSST;
using JavaSST.Parser.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Number = JavaSST.Parser.Models.Number;

namespace JavaSSTCompiler.Visualizer
{
  public class Visualizer
  {

    protected static readonly FontCollection _fontCollection = new FontCollection();
    protected static readonly Font _textFont;

    protected TextOptions TextOptions { get; init; } = new TextOptions(_textFont)
    {
      VerticalAlignment = VerticalAlignment.Center,
      HorizontalAlignment = HorizontalAlignment.Center,
      Dpi = 300,
      HintingMode= HintingMode.Standard,
      LayoutMode = LayoutMode.VerticalLeftRight,
      TextDirection = TextDirection.LeftToRight,
      TextAlignment = TextAlignment.Center,
    };

    protected float PaddingX { get; set; } = 16;
    protected float PaddingY { get; set; } = 16;

    protected float InterNodePaddingX { get; set; } = 40;
    protected float InterNodePaddingY { get; set; } = 40;

    static Visualizer()
    {
      var openSans = _fontCollection.Add("./Visualizer/OpenSans.ttf");
      _textFont =  openSans.CreateFont(18, FontStyle.Regular);
    }


    protected void determineSize(Node node)
    {
      var size = TextMeasurer.Measure(node.Text, new TextOptions(_textFont));
      node.Width = size.Width + PaddingX;
      node.Height= size.Height + PaddingY;
      node.TextMeasure = size;
    }

    protected void adjustX(Tree tree)
    {
      var nodes = tree.NodeLevels.Values.SelectMany(x => x).ToList();
      var minX = nodes.Min(x => x.X);
      nodes.ForEach(x => x.X += -minX);
    }

    public Node GenerateVisualTree(Class model)
    {
      var node = new Node($"Class\n{model.Identifier}");
      generateFinalFields(node, model.FinalFields);
      generateDynamicFields(node, model.DynamicFields);
      generateMethods(node, model.Methods);
      return node;
    }

    protected void generateMethods(Node parent, Method[] methods)
    {
      var methodNode = new Node("Methods", parent);
      foreach (var method in methods)
        generateMethod(methodNode, method);
    }

    protected void generateMethod(Node parent, Method method)
    {
      var methodNode = new Node("Method", parent);
      var signature = new Node("Signature", methodNode);
      new Node("public", signature);
      new Node(method.ReturnType.Value, signature);
      new Node(method.Identifier, signature);
      if (method.Parameters.Length > 0)
      {
        var parameters = new Node("Parameters", signature);
        foreach (var parameter in method.Parameters)
        {
          var parameterNode = new Node("Parameter");
          new Node(parameter.Type.Value, parameterNode);
          new Node(parameter.Identifier, parameterNode);
        }
      }

      generatedMethodBody(methodNode, method.Body);
    }

    protected void generatedMethodBody(Node parent, MethodBody body)
    {
      var bodyNode = new Node("Body", parent);
      generateLocalVariables(bodyNode, body.LocalVariables);
      generateStatements(bodyNode, body.Statements);
    }

    protected void generateStatements(Node parent, IStatement[] statements, string nodeName = "Statements")
    {
      var node = new Node(nodeName, parent);
      foreach (var statement in statements)
        generateStatement(node, statement);
    }

    protected void generateStatement(Node parent, IStatement statement)
    {
      switch (statement)
      {
        case ReturnStatement returnStatement:
          generateReturnStatement(parent, returnStatement);
          break;
        case Assignment assignment:
          generateAssignment(parent, assignment);
          break;
        case IfStatement ifStatement:
          generateIfStatement(parent, ifStatement);
          break;
        case WhileStatement whileStatement:
          generateWhileStatement(parent, whileStatement);
          break;
        case ProcedureCall call:
          generateProcedureCall(parent, call);
          break;
      }
    }

    protected void generateIfStatement(Node parent, IfStatement ifStatement)
    {
      var ifNode = new Node("IfStatement", parent);
      var conditionNode = new Node("Condition", ifNode);
      generateExpression(conditionNode, ifStatement.Condition);
      generateStatements(ifNode, ifStatement.ThenStatements, "Then");
      generateStatements(ifNode, ifStatement.ElseStatements, "Else");
    }

    protected void generateWhileStatement(Node parent, WhileStatement whileStatement)
    {
      var whileNode = new Node("WhileStatement", parent);
      var conditionNode = new Node("Condition", whileNode);
      generateExpression(conditionNode, whileStatement.Condition);
      generateStatements(parent, whileStatement.Statements);
    }

    protected void generateAssignment(Node parent, Assignment assignment)
    {
      var assignmentNode = new Node("Assignment", parent);
      new Node(assignment.Identifier, assignmentNode);
      new Node("=", assignmentNode);
      generateExpression(assignmentNode, assignment.Expression);
    }

    protected void generateReturnStatement(Node parent, ReturnStatement returnStatement)
    {
      var returnNode = new Node("return", parent);
      if (returnStatement.Expression != null)
        generateSimpleExpression(returnNode, returnStatement.Expression);
    }


    protected void generateLocalVariables(Node parent, DynamicField[] fields)
    {
      var fieldNode = new Node("Local Variables", parent);
      foreach (var field in fields)
        generateDynamicField(fieldNode, field);
    }


    protected void generateDynamicFields(Node parent, DynamicField[] fields)
    {
      var fieldNode = new Node("Dynamic Fields", parent);
      foreach (var field in fields)
        generateDynamicField(fieldNode, field);
    }

    protected void generateDynamicField(Node parent, DynamicField field)
    {
      var node = new Node("Dynamic Field", parent);
      new Node(field.Type.Value, node);
      new Node(field.Identifier, node);
    }

    protected void generateFinalFields(Node parent, FinalField[] finalFields)
    {
      var fieldNode = new Node("Final Fields", parent);
      foreach (var field in finalFields)
        generateFinalField(fieldNode, field);
    }

    protected void generateFinalField(Node parent, FinalField field)
    {
      var node = new Node($"Final Field", parent);
      new Node(field.Type.Value, node);
      new Node(field.Identifier, node);
      new Node("=", node);
      generateExpression(node, field.Expression);
    }

    protected void generateExpression(Node parent, Expression expression)
    {
      if (expression.Right == null)
      {
        generateSimpleExpression(parent, expression.Left);
      }
      else
      {
        var node = new Node("Expression", parent);
        generateSimpleExpression(node, expression.Left);
        new Node(expression.Comparison.Value.VisualiserName(), node);
        generateSimpleExpression(node, expression.Right);
      }
    }

    protected void generateSimpleExpression(Node parent, SimpleExpression expression)
    {
      if (expression.Terms.Count == 0)
      {
        generateTerm(parent, expression.Term);
      }
      else
      {
        var last = new Node("SimpleExpression", parent);
        var lastTerm = expression.Term;
        generateTerm(last, expression.Term);
        foreach (var term in expression.Terms)
        {
          last = new Node(term.Operator.VisualiserName(), last);
          generateTerm(last, lastTerm);
          lastTerm = term.Term;
        }
        generateTerm(last, lastTerm);
      }
    }


    protected void generateTerm(Node parent, Term term)
    {
      if (term.Factors.Count == 0)
      {
        generateFactor(parent, term.Factor);
      }
      else
      {
        var last = new Node("Factor", parent);
        var lastFactor = term.Factor;
        generateFactor(last, lastFactor);
        foreach (var factor in term.Factors)
        {
          last = new Node(factor.Type.VisualiserName(), last);
          generateFactor(last, lastFactor);
          lastFactor = factor.Factor;
        }
        generateFactor(last, lastFactor);
      }
    }


    protected void generateFactor(Node parent, IFactor factor)
    {
      switch (factor)
      {
        case Number number:
          new Node(number.Value.ToString(), parent);
          break;
        case Identifier identifier:
          new Node(identifier.Identifier, parent);
          break;
        case Expression expression:
          generateExpression(parent, expression);
          break;
        case ProcedureCall procedureCall:
          generateProcedureCall(parent, procedureCall);
          break;
      }
    }

    protected void generateProcedureCall(Node parent, ProcedureCall procedureCall)
    {
      var node = new Node($"{procedureCall.MethodSignature}", parent);
      foreach (var argument in procedureCall.Arguments)
        generateExpression(node, argument);
    }

    public void LayoutPass(Tree tree, bool centerParents = true)
    {
      var layerHeights = new Dictionary<int, float>();
      var yOff = 0F;
      foreach (var level in tree.NodeLevels)
      {
        layerHeights[level.Key] = yOff;
        yOff += level.Value.Max(x => x.Height) + InterNodePaddingY;
      }

      foreach (var level in tree.NodeLevels.Keys.Reverse())
      {
        var x = 0F;
        foreach (var node in tree.NodeLevels[level])
        {
          node.Y = layerHeights[level];
          node.X = x;
          x+= node.Width + InterNodePaddingX;
        }
      }

      if (centerParents)
      {
        foreach (var level in tree.NodeLevels.Keys.Reverse().Skip(1))
        {
          foreach (var node in tree.NodeLevels[level])
          {
            if (node.Childrens.Count > 0)
            {
              var minX = node.Childrens.Min(x => x.X);
              var maxX = node.Childrens.Max(x => x.X + x.Width);
              node.X = minX + ((maxX - minX) / 2) - (node.Width / 2);
            }
          }
        }
      }
    }

    public void Render(Class model, Stream fileOut)
    {
      var root = GenerateVisualTree(model);
      var tree = new Tree(root);
      tree.ForEach(determineSize);
      LayoutPass(tree, true);
      adjustX(tree);
      var size = tree.GetSize();
      using (var image = new Image<Rgba32>(size.Width, size.Height))
      {
        image.Mutate(x => renderNode(root, x));
        image.SaveAsPng(fileOut);
      }
    }

    protected void renderNode(Node node, IImageProcessingContext graphics)
    {
      graphics.Fill(Color.Black, new RectangleF(node.X, node.Y, node.Width, node.Height));
      graphics.DrawText(node.Text, _textFont, Color.White, node.TextLocation());
      foreach (var child in node.Childrens)
      {
        renderNode(child, graphics);
        graphics.DrawLines(Pens.Solid(Color.Black, 3), node.CenterBottom(), child.CenterTop());
      }
    }

  }
}

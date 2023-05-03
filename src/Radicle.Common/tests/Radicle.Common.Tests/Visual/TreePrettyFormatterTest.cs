namespace Radicle.Common.Visual;

using System;
using System.Collections.Generic;
using System.Linq;
using Radicle.Common.Extensions;
using Radicle.Common.Visual.Generic;
using Radicle.Common.Visual.Models;
using Xunit;

public class TreePrettyFormatterTest
{
    [Fact]
    public void Constructor_NullAdapter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TreePrettyFormatter<ITreeNode>(null!));
    }

    [Fact]
    public void ToLines_NullNode_Throws()
    {
        TreePrettyFormatter formatter = new();

        Assert.Throws<ArgumentNullException>(() => formatter.ToLines(null!));
    }

    [Fact]
    public void ToLines_ValidInput_Works()
    {
        Node node = new("root")
        {
            new Node()
            {
                new Node("a"),
                new Node("b"),
            },
            new Node("1"),
            new Node("2")
            {
                new Node("c"),
                new Node(),
            },
            new Node("3")
            {
                new Node("d\n(note)"),
                new Node("e")
                {
                    new Node("1a"),
                    new Node("1b"),
                },
            },
            new Node("4")
            {
                new Node(),
                new Node("c"),
                new Node(),
            },
        };

        string[] expected = @"
- root
  +-+- a
  | `- b
  +- 1
  +- 2
  |  +- c
  |  `- ?
  +- 3
  |  +- d
  |  |  (note)
  |  `- e
  |     +- 1a
  |     `- 1b
  `- 4
     +- ?
     +- c
     `- ?".ToLines(StringSplitOptions.RemoveEmptyEntries);

        TreePrettyFormatter formatter = new();
        IEnumerable<string> actual = formatter.ToLines(node);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToLines_LeafOnlyLabels_Works()
    {
        Node node = new()
        {
            new Node()
            {
                new Node()
                {
                    new Node("a"),
                    new Node("b"),
                },
                new Node()
                {
                    new Node()
                    {
                        new Node("c"),
                    },
                },
                new Node("d"),
            },
        };

        string[] expected = @"
-~-+-+- a
   | `- b
   +-~-~- c
   `- d".ToLines(StringSplitOptions.RemoveEmptyEntries);

        TreePrettyFormatter formatter = new();
        IEnumerable<string> actual = formatter.ToLines(node);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToLines_MultilineLabels_Works()
    {
        Node node = new("root\nspanning\nmany\nlines")
        {
            new Node()
            {
                new Node("sub\nlabel")
                {
                    new Node("multiline\nlabel"),
                    new Node("multiline\nlabel"),
                },
                new Node()
                {
                    new Node()
                    {
                        new Node(".\n."),
                    },
                },
                new Node("multiline\nlabel\nleaf"),
            },
        };

        string[] expected = @"
- root
  spanning
  many
  lines
  `-+- sub
    |  label
    |  +- multiline
    |  |  label
    |  `- multiline
    |     label
    +-~-~- .
    |      .
    `- multiline
       label
       leaf".ToLines(StringSplitOptions.RemoveEmptyEntries);

        TreePrettyFormatter formatter = new();
        IEnumerable<string> actual = formatter.ToLines(node);
        string lines = string.Join(Environment.NewLine, actual);

        Assert.NotNull(lines);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToAnnotatedLines_NullNode_Throws()
    {
        TreePrettyFormatter formatter = new();

        Assert.Throws<ArgumentNullException>(() => formatter.ToAnnotatedLines(null!));
    }

    [Fact]
    public void ToAnnotatedLines_WithCustomNodes_PassesInformation()
    {
        Node node = new("root")
        {
            new Node("level a")
            {
                new Node("sub a"),
                new Node("sub b"),
            },
            new Node("level b")
            {
                new Node("sub c")
                {
                    new Node("base a\n(comment)"),
                },
            },
            new Node("level c"),
        };

        string[] expected = @"
- root               | 3
  +- level a         | 2
  |  +- sub a        | 0]
  |  `- sub b        | 0]
  +- level b         | 1
  |  `- sub c        | 1
  |     `- base a    | 0]
  |        (comment) |
  `- level c         | 0]".ToLines(StringSplitOptions.RemoveEmptyEntries);

        TreePrettyFormatter formatter = new();
        AnnotatedLine<ITreeNode>[] annotated = formatter
                .ToAnnotatedLines(node)
                .ToArray();

        int maxlineLength = annotated.Max(n => n.Line.Length);

        IEnumerable<string> actual = annotated
                .Select(a => a.Line.PadRight(maxlineLength)
                        + " |"
                        + (a.IsContinuation ? string.Empty : $" {a.Path.Node.ChildCount}")
                        + (a.IsLeaf && !a.IsContinuation ? "]" : string.Empty));

        Assert.Equal(expected, actual);
    }

    private class Node : List<ITreeNode>, ITreeNode
    {
        public Node(string? label = null)
        {
            if (label is not null)
            {
                this.Label.AddRange(label.ToLines().Select(l => (LabelLine)l));
            }
        }

        public List<LabelLine> Label { get; set; } = new List<LabelLine>();

        public bool IsLeaf => this.Count == 0;

        public int ChildCount => this.Count;

        IEnumerable<ITreeNode> ITreeNode.Children => this;

        IEnumerable<LabelLine> ITreeNode.Label => this.Label;
    }
}

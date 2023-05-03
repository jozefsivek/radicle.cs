namespace Radicle.Common.Visual.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Visual.Models;

/// <summary>
/// Utility class for writing ASCII-style trees.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public class TreePrettyFormatter<TNode>
{
    private readonly ITreeAdapter<TNode> adapter;

    private readonly string separatorLabel = " ";

    private readonly LabelLine labelLessLeaf = (LabelLine)"?";

    private readonly string bulletLabel = "-";

    private readonly string bulletFirstChild = "+";

    private readonly string bulletChild = "+";

    private readonly string bulletLabelLessParentLastChild = "~";

    private readonly string bulletLastChild = "`";

    private readonly string ghostBulletLabel = " ";

    private readonly string ghostBulletChild = "|";

    private readonly string ghostBulletLastChild = " ";

    /// <summary>
    /// Initializes a new instance of the <see cref="TreePrettyFormatter{TNode}"/> class.
    /// </summary>
    /// <param name="adapter">Adapter for the node.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TreePrettyFormatter(
            ITreeAdapter<TNode> adapter)
    {
        this.adapter = Ensure.Param(adapter).Value;
    }

    /// <summary>
    /// Convert given <paramref name="node"/> to
    /// printable lines with no line breaks.
    /// </summary>
    /// <param name="node">Node to convert.</param>
    /// <returns>Enumeration of the lines which
    /// can be printed.</returns>
    public IEnumerable<string> ToLines(TNode node)
    {
        Ensure.Param(node).Done();

        IEnumerable<string> Result()
        {
            foreach ((StringBuilder builder, _, _, _) in
                    this.ConvertToLines(new TreePath<TNode>(node)))
            {
                yield return builder.ToString();
            }
        }

        return Result();
    }

    /// <summary>
    /// Convert given <paramref name="node"/> to
    /// printable lines with no line breaks.
    /// </summary>
    /// <param name="node">Node to convert.</param>
    /// <returns>Enumeration of the lines which
    /// can be printed.</returns>
    public IEnumerable<AnnotatedLine<TNode>> ToAnnotatedLines(TNode node)
    {
        Ensure.Param(node).Done();

        IEnumerable<AnnotatedLine<TNode>> Result()
        {
            foreach ((StringBuilder builder, TreePath<TNode> path, bool leaf, bool continuation) in
                    this.ConvertToLines(new TreePath<TNode>(node)))
            {
                yield return new AnnotatedLine<TNode>(
                        path,
                        builder.ToString(),
                        isLeaf: leaf,
                        isContunuation: continuation);
            }
        }

        return Result();
    }

    /* Here we use tuple because this annotation information
     * is just passed internally and discarted quickly */
    private IEnumerable<(StringBuilder Builder, TreePath<TNode> Path, bool Leaf, bool Continuation)>
            ConvertToLines(
                TreePath<TNode> path)
    {
        bool labelBulletExhausted = false;

        // node label
        LabelLine[] labelLines = this.adapter.GetLabel(path).ToArray();
        bool nodeIsLeaf = this.adapter.IsLeaf(path);

        if (labelLines.Length == 0 && nodeIsLeaf)
        {
            labelLines = new[] { this.labelLessLeaf };
        }

        if (labelLines.Length > 0)
        {
            foreach (string labelLine in labelLines)
            {
                StringBuilder sb = new();
                bool continuation = labelBulletExhausted;

                if (labelBulletExhausted)
                {
                    sb = sb.Append(this.ghostBulletLabel)
                            .Append(this.separatorLabel);
                }
                else
                {
                    sb = sb.Append(this.bulletLabel)
                            .Append(this.separatorLabel);
                    labelBulletExhausted = true;
                }

                yield return (sb.Append(labelLine), path, nodeIsLeaf, continuation);
            }
        }

        StringBuilder childPrefixBuilder = new StringBuilder()
                .Append(this.ghostBulletLabel);

        if (labelBulletExhausted)
        {
            childPrefixBuilder = childPrefixBuilder.Append(this.separatorLabel);
        }

        string commonChildPrefix = childPrefixBuilder.ToString();
        int childIndex = 0;
        int childCount = this.adapter.GetChildCount(path);

        // children
        foreach (TNode child in this.adapter.GetChildren(path))
        {
            Ensure.Param(child).Done();

            childIndex++;
            bool isLastChild = childIndex >= childCount;
            bool isFirstChild = childIndex == 1;
            bool isChildLeadLine = true;

            TreePath<TNode> subPath = new(child, parent: path);

            foreach ((StringBuilder cBuilder, TreePath<TNode> cPath, bool cLeaf, bool cContinuation) in
                    this.ConvertToLines(subPath))
            {
                StringBuilder sb = cBuilder;

                // prefix child lines with child topology bullets
                if (isChildLeadLine)
                {
                    if (isLastChild && !labelBulletExhausted)
                    {
                        sb = sb.Insert(0, this.bulletLabelLessParentLastChild);
                    }
                    else if (isLastChild)
                    {
                        sb = sb.Insert(0, this.bulletLastChild);
                    }
                    else if (isFirstChild)
                    {
                        sb = sb.Insert(0, this.bulletFirstChild);
                    }
                    else
                    {
                        sb = sb.Insert(0, this.bulletChild);
                    }

                    isChildLeadLine = false;
                }
                else if (isLastChild)
                {
                    sb = sb.Insert(0, this.ghostBulletLastChild);
                }
                else
                {
                    sb = sb.Insert(0, this.ghostBulletChild);
                }

                // finally prefix child lines with common prefix
                if (labelBulletExhausted)
                {
                    sb = sb.Insert(0, commonChildPrefix);
                }
                else
                {
                    // for a case when parent did not have label,
                    // the first line of first child is carrying
                    // the label bullet
                    sb = sb.Insert(0, this.bulletLabel);
                    labelBulletExhausted = true;
                }

                yield return (sb, cPath, cLeaf, cContinuation);
            }
        }
    }
}

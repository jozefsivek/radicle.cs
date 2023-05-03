namespace Radicle.Common.Visual.Models;

using System.Collections.Generic;

/// <summary>
/// General interface of the tree node for visualization purposes.
/// </summary>
public interface ITreeNode
{
    /// <summary>
    /// Gets enumeration of child nodes of this node.
    /// </summary>
    IEnumerable<ITreeNode> Children { get; }

    /// <summary>
    /// Gets label of this node as individual lines. Can be empty.
    /// </summary>
    IEnumerable<LabelLine> Label { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// node is a leaf with no children.
    /// </summary>
    public bool IsLeaf { get; }

    /// <summary>
    /// Gets amount of children.
    /// </summary>
    public int ChildCount { get; }
}

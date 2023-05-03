namespace Radicle.Common.Visual.Models;

using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Default tree adapter for <see cref="ITreeNode"/>.
/// </summary>
public sealed class DefaultTreeAdapter : ITreeAdapter<ITreeNode>
{
    /// <inheritdoc/>
    public int GetChildCount(
            TreePath<ITreeNode> path)
    {
        return Ensure.Param(path).Value.Node.ChildCount;
    }

    /// <inheritdoc/>
    public IEnumerable<ITreeNode> GetChildren(
            TreePath<ITreeNode> path)
    {
        return Ensure.Param(path).Value.Node.Children;
    }

    /// <inheritdoc/>
    public bool IsLeaf(
            TreePath<ITreeNode> path)
    {
        return Ensure.Param(path).Value.Node.IsLeaf;
    }

    /// <inheritdoc/>
    public IEnumerable<LabelLine> GetLabel(
            TreePath<ITreeNode> path)
    {
        return Ensure.Param(path).Value.Node.Label;
    }
}

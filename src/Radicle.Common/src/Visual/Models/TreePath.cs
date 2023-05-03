namespace Radicle.Common.Visual.Models;

using System;
using System.Collections.Generic;
using Radicle.Common.Check;

/// <summary>
/// Immutable path used to give exact position in the tree.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public sealed class TreePath<TNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreePath{TNode}"/> class.
    /// </summary>
    /// <param name="node">Terminal node for this path.</param>
    /// <param name="parent">Optional parent path.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public TreePath(
            TNode node,
            TreePath<TNode>? parent = null)
    {
        this.Node = Ensure.Param(node).Value;
        this.Parent = parent;
    }

    /// <summary>
    /// Gets the terminal node of this path.
    /// </summary>
    public TNode Node { get; }

    /// <summary>
    /// Gets optional parent path of this <see cref="Node"/>.
    /// </summary>
    public TreePath<TNode>? Parent { get; }

    /// <summary>
    /// Enumerate nodes starting from the root
    /// to this <see cref="Node"/>.
    /// </summary>
    /// <returns>Enumeration of nodes.</returns>
    public IEnumerable<TNode> GetAncestorFirstChain()
    {
        if (this.Parent is not null)
        {
            foreach (TNode node in this.Parent.GetAncestorFirstChain())
            {
                yield return node;
            }
        }

        yield return this.Node;
    }
}

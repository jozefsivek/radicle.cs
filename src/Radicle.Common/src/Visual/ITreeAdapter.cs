namespace Radicle.Common.Visual;

using System;
using System.Collections.Generic;
using Radicle.Common.Visual.Models;

/// <summary>
/// Interface of the tree adapter which can be used to convert
/// arbitrary tree like object to printable tree.
/// </summary>
/// <remarks>The methods are driven by the
///     <see cref="TreePath{TNode}"/> so that one
///     have always access to ancestors node as well
///     when constructing labels etc.</remarks>
/// <typeparam name="TNode">Type of the node.</typeparam>
public interface ITreeAdapter<TNode>
{
    /// <summary>
    /// Get enumeration of the children for the node.
    /// </summary>
    /// <param name="path">Path to node.</param>
    /// <returns>Enumeration of children.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IEnumerable<TNode> GetChildren(
            TreePath<TNode> path);

    /// <summary>
    /// Gets amount of children returned by
    /// <see cref="GetChildren(TreePath{TNode})"/>.
    /// </summary>
    /// <param name="path">Path to node.</param>
    /// <returns>Amount of children.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    public int GetChildCount(
            TreePath<TNode> path);

    /// <summary>
    /// Get label for the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">Path to node to get label for.</param>
    /// <returns>Enumeraion of label lines. Can be empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    IEnumerable<LabelLine> GetLabel(
            TreePath<TNode> path);

    /// <summary>
    /// Gets value indicating whether given <paramref name="path"/>
    /// is a leaf node with no child nodes.
    /// </summary>
    /// <param name="path">Path to node to probe.</param>
    /// <returns><see langword="true"/> if leaf;
    /// <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    bool IsLeaf(TreePath<TNode> path);
}

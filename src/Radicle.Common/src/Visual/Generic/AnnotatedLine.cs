namespace Radicle.Common.Visual.Generic;

using System;
using Radicle.Common.Check;
using Radicle.Common.Visual.Models;

/// <summary>
/// Immutable model for annotated line of tree pretty formatter.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public sealed class AnnotatedLine<TNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotatedLine{TNode}"/> class.
    /// </summary>
    /// <param name="path">Path of the current line.</param>
    /// <param name="line">Line.</param>
    /// <param name="isLeaf">Value indicating this
    ///     line corresponds to leaf node.</param>
    /// <param name="isContunuation">Value indicating whether this
    ///     line is continuation of previous with the
    ///     same path, e.g. for multi-line labels.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown
    ///     if <paramref name="line"/> contains new lines.</exception>
    public AnnotatedLine(
            TreePath<TNode> path,
            string line,
            bool isLeaf = false,
            bool isContunuation = false)
    {
        this.Path = Ensure.Param(path).Value;
        this.Line = Ensure.Param(line).NoNewLines().Value;
        this.IsLeaf = isLeaf;
        this.IsContinuation = isContunuation;
    }

    /// <summary>
    /// Gets path of the current line.
    /// </summary>
    public TreePath<TNode> Path { get; }

    /// <summary>
    /// Gets the line string value.
    /// </summary>
    public string Line { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="Path"/> points to leaf node.
    /// </summary>
    public bool IsLeaf { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// line is continuation of previous with the
    /// same path, e.g. for multi-line labels.
    /// </summary>
    public bool IsContinuation { get; }
}

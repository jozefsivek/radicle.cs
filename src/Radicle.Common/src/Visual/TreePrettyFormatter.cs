namespace Radicle.Common.Visual;

using Radicle.Common.Visual.Generic;
using Radicle.Common.Visual.Models;

/// <summary>
/// Utility class for writing ASCII-style trees.
/// </summary>
public sealed class TreePrettyFormatter : TreePrettyFormatter<ITreeNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreePrettyFormatter"/> class.
    /// </summary>
    public TreePrettyFormatter()
        : base(new DefaultTreeAdapter())
    {
    }
}

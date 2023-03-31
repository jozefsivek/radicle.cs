namespace Radicle.Alike.Redis;

using System;
using System.IO;
using Radicle.Common.Base;
using Radicle.Common.Check;

/* TODO: profile this implementation against the one with spans */

/// <summary>
/// Base class of RESP readers and writers.
/// </summary>
public abstract class RESPStreamWrapper : AsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPStreamWrapper"/> class.
    /// </summary>
    /// <remarks>Note that given <paramref name="stream"/>
    /// is automatically disposed when this instance is
    /// disposed.</remarks>
    /// <param name="stream">Stream.</param>
    /// <param name="leaveOpen">Flag specifying to leave
    ///     <paramref name="stream"/> open (and hence not disposed)
    ///     when disposing this instance. Defaults to
    ///     <see langword="false"/> so the <paramref name="stream"/>
    ///     is disposed when this instance is disposed.</param>
    /// <param name="protocolVersion">Protocol version to use.
    ///     Defaults to latest.</param>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if any required argument is <see langword="null"/>.</exception>
    protected internal RESPStreamWrapper(
            Stream stream,
            bool leaveOpen = false,
            RESPVersions protocolVersion = RESPVersions.RESP3)
    {
        this.Stream = Ensure.Param(stream).Value;
        this.LeaveOpen = leaveOpen;
        this.ProtocolVersion = protocolVersion;

        if (!this.LeaveOpen)
        {
            // instead of calling exlicit Close() we will dispose,
            // calling just Close() implies knowledge of the implementation!
            this.Add(this.Stream);
        }
    }

    /// <summary>
    /// Gets or sets protocol version to use. The version can be changed on the fly.
    /// </summary>
    public RESPVersions ProtocolVersion { get; set; }

    /// <summary>
    /// Gets underlying stream.
    /// </summary>
    protected Stream Stream { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Stream"/>
    /// should be disposed when disposing this instance.
    /// See https://github.com/dotnet/corefx/blob/09e2417cd0505df4558535651efb1bbcffdf0c59/src/Common/src/CoreLib/System/IO/BinaryWriter.cs#L80 .
    /// </summary>
    protected bool LeaveOpen { get; }
}

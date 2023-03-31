namespace Radicle.Alike.Redis;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Slow memory stream is a mock class which mimicks specific behaviour
/// of the streams where not all requested byte length of data is returned at ones.
/// </summary>
public class ChunkedMemoryStream : MemoryStream
{
    private readonly int readLimit;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChunkedMemoryStream"/> class.
    /// </summary>
    /// <param name="buffer">Init buffer for memory stream.</param>
    /// <param name="readLimit">Maximum amount of bytes to return
    ///     in one read.</param>
    public ChunkedMemoryStream(
            byte[] buffer,
            int readLimit = 10)
        : base(buffer)
    {
        this.readLimit = readLimit;
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        int total = 0;
        int writeIndex = offset;

        for (int i = 0; i < this.readLimit && i < count; i++)
        {
            int b = this.ReadByte();

            if (b == -1)
            {
                return total;
            }

            buffer[writeIndex++] = (byte)b;
            total++;
        }

        return total;
    }

    /// <inheritdoc />
    public override async Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
    {
        int total = 0;
        int writeIndex = offset;

        for (int i = 0; i < this.readLimit && i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();

            int b = this.ReadByte();

            if (b == -1)
            {
                return total;
            }

            buffer[writeIndex++] = (byte)b;
            total++;
        }

        return total;
    }
}

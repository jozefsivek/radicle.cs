namespace Radicle.Common.IO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Radicle.Common.Check;
using Radicle.Common.Compression;
using Xunit;

/*
File system is required for these tests,
auto-cleanup is implemented.
*/

public class FlexFileTest
{
    [Fact]
    public async Task ReadAllTextAsync_WriteStream_Throws()
    {
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();

        using FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fw.ReadAllTextAsync())
                .ConfigureAwait(false);
    }

    [Theory]
    [InlineData(".gz")]
    [InlineData(".z")]
    public async Task ReadAllTextAsync_CorruptStream_Throws(string extension)
    {
        const string testString = "hello test";
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();
        byte[] payload = Encoding.UTF8.GetBytes(testString);

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            await ((Stream)fw).WriteAsync(payload).ConfigureAwait(false);
        }

        File.Move(fileName, fileName + extension, overwrite: true);

        using FlexFile cfr = FlexFile.Open(
                fileName + extension,
                FileMode.OpenOrCreate,
                FileAccess.Read);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
                cfr.ReadAllTextAsync()).ConfigureAwait(false);
    }

    [Theory]
    [InlineData(".gz")]
    [InlineData(".z")]
    public async Task ReadAllBytes_CorruptStream_Throws(string extension)
    {
        const string testString = "hello test";
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();
        byte[] payload = Encoding.UTF8.GetBytes(testString);

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            await ((Stream)fw).WriteAsync(payload).ConfigureAwait(false);
        }

        File.Move(fileName, fileName + extension, overwrite: true);

        using FlexFile cfr = FlexFile.Open(
                fileName + extension,
                FileMode.OpenOrCreate,
                FileAccess.Read);

        Assert.Throws<InvalidDataException>(() =>
                cfr.ReadAllBytes());
    }

    [Fact]
    public void ReadAllBytes_WriteStream_Throws()
    {
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();

        using FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write);

        Assert.Throws<InvalidOperationException>(() => fw.ReadAllBytes());
    }

    [Fact]
    public async Task ToStream_RegularFileWite_Works()
    {
        const string testString = "hello test";
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();
        byte[] payload = Encoding.UTF8.GetBytes(testString);

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            await ((Stream)fw).WriteAsync(payload).ConfigureAwait(false);
        }

        using FlexFile fr = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Read);

        Assert.False(fr.IsCompressed);
        Assert.Equal(CompressionFormat.None, fr.CompressionFormat);
        Assert.Equal(testString, await fr.ReadAllTextAsync().ConfigureAwait(false));
    }

    [Theory]
    [InlineData(".gz")]
    [InlineData(".z")]
    public async Task ToStream_GzipFileWite_Works(string extension)
    {
        const string testString = "hello test";
        using TmpDir dir = new();
        string fileName = dir.GetTempFile(extension);
        byte[] payload = Encoding.UTF8.GetBytes(testString);

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            await ((Stream)fw).WriteAsync(payload).ConfigureAwait(false);
        }

        using FlexFile fr = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Read);

        Assert.True(fr.IsCompressed);
        Assert.Equal(CompressionFormat.Gzip, fr.CompressionFormat);
        Assert.Equal(testString, await fr.ReadAllTextAsync().ConfigureAwait(false));
    }

    [Fact]
    public void TryGetPossiblePath_NullPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => FlexFile.TryGetPossiblePath(null!, out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(".gz")]
    [InlineData(".z")]
    public async Task TryGetPossiblePath_ValidPath_ReturnsExistingPath(string? extension)
    {
        const string testString = "hello test";
        using TmpDir dir = new();
        string fileName = dir.GetTempFile(extension);
        byte[] payload = Encoding.UTF8.GetBytes(testString);

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            await ((Stream)fw).WriteAsync(payload).ConfigureAwait(false);
        }

        int ext = extension?.Length ?? 0;

        Assert.True(FlexFile.TryGetPossiblePath(
                fileName[..^ext],
                out string? name));
        Assert.Equal(fileName, name);
    }

    [Fact]
    public void TryGetPossiblePath_NonExistingPath_ReturnsFalse()
    {
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();

        Assert.False(FlexFile.TryGetPossiblePath(fileName, out _));
    }

#pragma warning disable xUnit1004 // Test methods should not be skipped
    [Fact(Skip = "not for automatesd tests")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
    public void Performance()
    {
        using TmpDir dir = new();
        string fileName = dir.GetTempFile();
        string cfileName = dir.GetTempFile(".gz");
        Stopwatch sw = new();
        Stopwatch csw = new();
        byte[] payload = Enumerable.Range(0, byte.MaxValue).Select(i => (byte)i).ToArray();
        int length = 100_000_000 / payload.Length;

        using (FlexFile fw = FlexFile.Open(
                fileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            sw.Start();

            for (int i = 0; i < length; i++)
            {
                ((Stream)fw).Write(payload, 0, payload.Length);
            }

            sw.Stop();
        }

        using (FlexFile cfw = FlexFile.Open(
                cfileName,
                FileMode.OpenOrCreate,
                FileAccess.Write))
        {
            csw.Start();

            for (int i = 0; i < length; i++)
            {
                cfw.ToStream().Write(payload, 0, payload.Length);

                cfw.ToStream().Flush();
            }

            csw.Stop();
        }

        string timing = $"Normal: {sw.Elapsed.TotalSeconds}; compressed: {csw.Elapsed.TotalSeconds}";

        Assert.NotNull(timing);
    }

    private sealed class TmpDir : IDisposable
    {
        private readonly string dirPath;

        private readonly string prefix;

        private readonly List<string> files = new();

        private long counter;

        public TmpDir(string? directory = null)
        {
            this.dirPath = directory ?? Path.GetTempPath();
            this.prefix = CompactUUID.GenerateWebSafe();
        }

        internal bool Disposed { get; set; }

        public string GetTempFile(string? suffixExtension = null)
        {
            string suffix = Ensure.Optional(suffixExtension)
                    .NoNewLines()
                    .NotWhiteSpace()
                    .ValueOr(string.Empty);

            while (true)
            {
                long index = Interlocked.Increment(ref this.counter);
                string fileName = $"temp_file_{this.prefix}-{index}.ext{suffix}";
                string filePath = Path.Join(this.dirPath, fileName);

                if (!File.Exists(filePath))
                {
                    this.files.Add(filePath);

                    return filePath;
                }
            }
        }

        public void Dispose()
        {
            if (this.Disposed)
            {
                return;
            }

            foreach (string filePath in this.files)
            {
                File.Delete(filePath);
            }

            this.Disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}

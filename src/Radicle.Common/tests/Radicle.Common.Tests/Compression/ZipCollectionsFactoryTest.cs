namespace Radicle.Common.Compression;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public class ZipCollectionsFactoryTest
{
    private const int N = 100_000;
    private const ZipCollectionsLevel Level = ZipCollectionsLevel.High;

    /* private static readonly Func<int, string> valueFactory = i => $"a value under index {i}"; */
    private static readonly Func<int, string> valueFactory = i => $"a value under index {i}: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec a felis eros. Mauris vitae ultricies lectus. Phasellus quis ex vehicula, dapibus libero sed, pretium libero. Duis pharetra lorem nulla, in feugiat orci mattis ut. Mauris dictum ornare sem, varius dapibus dolor pellentesque eget.";

    private readonly ITestOutputHelper output;

    public ZipCollectionsFactoryTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Create_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                ZipCollectionsFactory.Default.Create(
                    (IEnumerable<KeyValuePair<string, object>>)null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidInput_ProducesExpectedDictionary(
            int length)
    {
        List<KeyValuePair<int, string>> data = new(length);

        for (int i = 0; i < length; i++)
        {
            data.Add(new KeyValuePair<int, string>(i, $"data {i}"));
        }

        IImmutableDictionary<int, string> result =
                ZipCollectionsFactory.Default.Create(data);

        Assert.Equal(length, result.Count);
        Assert.Equal(data, result);

        Assert.Equal(data.Select(kv => kv.Key), result.Keys);
        Assert.Equal(data.Select(kv => kv.Value), result.Values);

        foreach (KeyValuePair<int, string> kv in data)
        {
            Assert.True(result.ContainsKey(kv.Key));
            Assert.Equal(kv.Value, result[kv.Key]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_ValidInput_ProducesFunctionalDictionary(
            int length)
    {
        ImmutableDictionary<int, string>.Builder dataBuilder =
                ImmutableDictionary.CreateBuilder<int, string>();

        for (int i = 0; i < length; i++)
        {
            dataBuilder.Add(i, $"data {i}");
        }

        ImmutableDictionary<int, string> data = dataBuilder.ToImmutable();

        IImmutableDictionary<int, string> result =
                ZipCollectionsFactory.Default.Create(data);

        Assert.Equal(data, result);

        data = data.Add(-1, "new");
        result = result.Add(-1, "new");

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.AddRange(new[]
        {
            new KeyValuePair<int, string>(-1, "new"),
            new KeyValuePair<int, string>(-2, "new2")
        });
        result = result.AddRange(new[]
        {
            new KeyValuePair<int, string>(-1, "new"),
            new KeyValuePair<int, string>(-2, "new2")
        });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.SetItems(new[]
        {
            new KeyValuePair<int, string>(-1, "@new"),
            new KeyValuePair<int, string>(-2, "@new2")
        });
        result = result.SetItems(new[]
        {
            new KeyValuePair<int, string>(-1, "@new"),
            new KeyValuePair<int, string>(-2, "@new2")
        });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.RemoveRange(new[] { -1, -2 });
        result = result.RemoveRange(new[] { -1, -2 });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.Clear();
        result = result.Clear();

        Assert.Equal(data.ToHashSet(), result.ToHashSet());
    }

    [Fact]
    public void CreateCompressed_NullDictionary_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                ZipCollectionsFactory.Default.CreateCompressed(
                    (IEnumerable<KeyValuePair<string, string>>)null!));
    }

    [Fact]
    public void CreateCompressed_NullCollection_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
                ZipCollectionsFactory.Default.CreateCompressed(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateCompressed_ValidInput_ProducesExpectedDictionary(
            int length)
    {
        List<KeyValuePair<int, string>> data = new(length);

        for (int i = 0; i < length; i++)
        {
            data.Add(new KeyValuePair<int, string>(i, $"data {i}"));
        }

        IImmutableDictionary<int, string> result =
                ZipCollectionsFactory.Default.CreateCompressed(data);

        Assert.Equal(length, result.Count);
        Assert.Equal(data, result);

        Assert.Equal(data.Select(kv => kv.Key), result.Keys);
        Assert.Equal(data.Select(kv => kv.Value), result.Values);

        foreach (KeyValuePair<int, string> kv in data)
        {
            Assert.True(result.ContainsKey(kv.Key));
            Assert.Equal(kv.Value, result[kv.Key]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateCompressed_ValidInput_ProducesFunctionalDictionary(
            int length)
    {
        ImmutableDictionary<int, string>.Builder dataBuilder =
                ImmutableDictionary.CreateBuilder<int, string>();

        for (int i = 0; i < length; i++)
        {
            dataBuilder.Add(i, $"data {i}");
        }

        ImmutableDictionary<int, string> data = dataBuilder.ToImmutable();

        IImmutableDictionary<int, string> result =
                ZipCollectionsFactory.Default.CreateCompressed(data);

        Assert.Equal(data, result);

        data = data.Add(-1, "new");
        result = result.Add(-1, "new");

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.AddRange(new[]
        {
            new KeyValuePair<int, string>(-1, "new"),
            new KeyValuePair<int, string>(-2, "new2")
        });
        result = result.AddRange(new[]
        {
            new KeyValuePair<int, string>(-1, "new"),
            new KeyValuePair<int, string>(-2, "new2")
        });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.SetItems(new[]
        {
            new KeyValuePair<int, string>(-1, "@new"),
            new KeyValuePair<int, string>(-2, "@new2")
        });
        result = result.SetItems(new[]
        {
            new KeyValuePair<int, string>(-1, "@new"),
            new KeyValuePair<int, string>(-2, "@new2")
        });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.RemoveRange(new[] { -1, -2 });
        result = result.RemoveRange(new[] { -1, -2 });

        Assert.Equal(data.ToHashSet(), result.ToHashSet());

        data = data.Clear();
        result = result.Clear();

        Assert.Equal(data.ToHashSet(), result.ToHashSet());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateCompressed_ValidInput_ProducesExpectedCollection(
            int length)
    {
        List<string> data = new(length);

        for (int i = 0; i < length; i++)
        {
            data.Add($"data {i}");
        }

        IImmutableList<string> result =
                ZipCollectionsFactory.Default.CreateCompressed(data);

        Assert.Equal(length, result.Count);
        Assert.Equal(data, result);

        for (int i = 0; i < data.Count; i++)
        {
            Assert.Equal(data[i], result[i]);
            Assert.Equal(i, result.IndexOf(data[i]));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateCompressed_ValidInput_ProducesFunctionalCollection(
            int length)
    {
        ImmutableList<string>.Builder dataBuilder =
                ImmutableList.CreateBuilder<string>();

        for (int i = 0; i < length; i++)
        {
            dataBuilder.Add($"data {i}");
        }

        ImmutableList<string> data = dataBuilder.ToImmutable();

        IImmutableList<string> result =
                ZipCollectionsFactory.Default.CreateCompressed(data);

        Assert.Equal(data, result);

        data = data.AddRange(new[] { "new", "new2" });
        result = result.AddRange(new[] { "new", "new2" });

        Assert.Equal(data, result);

        data = data.InsertRange(0, new[] { "inset 0", "insert 1", "foo", "bar" });
        result = result.InsertRange(0, new[] { "inset 0", "insert 1", "foo", "bar" });

        Assert.Equal(data, result);

        data = data.InsertRange(data.Count - 1, new[] { "inset tail", "insert 2 tail" });
        result = result.InsertRange(result.Count - 1, new[] { "inset tail", "insert 2 tail" });

        Assert.Equal(data, result);

        data = data.RemoveAll(s => s.Contains(" 2 ", StringComparison.Ordinal));
        result = result.RemoveAll(s => s.Contains(" 2 ", StringComparison.Ordinal));

        Assert.Equal(data, result);

        data = data.RemoveAt(0);
        result = result.RemoveAt(0);

        Assert.Equal(data, result);

        data = data.RemoveRange(new[] { "inset 0", "insert 1" }, StringComparer.Ordinal);
        result = result.RemoveRange(new[] { "inset 0", "insert 1" }, StringComparer.Ordinal);

        Assert.Equal(data, result);

        data = data.RemoveRange(0, 1);
        result = result.RemoveRange(0, 1);

        Assert.Equal(data, result);

        data = data.Replace("bar", "Foo", StringComparer.Ordinal);
        result = result.Replace("bar", "Foo", StringComparer.Ordinal);

        Assert.Equal(data, result);

        data = data.SetItem(data.Count - 1, "set");
        result = result.SetItem(result.Count - 1, "set");

        Assert.Equal(data, result);

        data = data.Clear();
        result = result.Clear();

        Assert.Equal(data, result);
    }

#pragma warning disable xUnit1004 // Test methods should not be skipped
    [Theory(Skip = "performance test")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Create_GenericDictionary_ProducesMemoryEfficientObject(
            int cardinality)
    {
        long mem = GC.GetTotalMemory(forceFullCollection: true);

        ZipCollectionsFactory factory = new(Level);

        this.output.WriteLine("Generic dictionary test");
        this.output.WriteLine($"Level: {factory.Level}");
        this.output.WriteLine($"Objects: {N}, of length: {valueFactory(0).Length}");
        this.output.WriteLine($"Cardinality: {cardinality}");
        this.output.WriteLine($"Memory usage beginning: {mem}B");

        object[] stash = new object[N];

        for (int i = 0; i < N; i++)
        {
            stash[i] = factory.Create(
                    Enumerable
                        .Range(0, cardinality)
                        .Select(i => new KeyValuePair<int, string>(
                            i,
                            valueFactory(i))));
        }

        long mem2 = GC.GetTotalMemory(forceFullCollection: true);
        this.output.WriteLine($"Memory usage end: {mem}B");
        this.output.WriteLine($"Memory delta: {mem2 - mem}B");
    }

    [Theory(Skip = "performance test")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Create_StringDictionary_ProducesMemoryEfficientObject(
            int cardinality)
    {
        long mem = GC.GetTotalMemory(forceFullCollection: true);

        ZipCollectionsFactory factory = new(Level);

        this.output.WriteLine("String compressed dictionary test");
        this.output.WriteLine($"Level: {factory.Level}");
        this.output.WriteLine($"Objects: {N}, of length: {valueFactory(0).Length}");
        this.output.WriteLine($"Cardinality: {cardinality}");
        this.output.WriteLine($"Memory usage beginning: {mem}B");

        object[] stash = new object[N];

        for (int i = 0; i < N; i++)
        {
            stash[i] = factory.CreateCompressed(
                    Enumerable
                        .Range(0, cardinality)
                        .Select(i => new KeyValuePair<int, string>(
                            i,
                            valueFactory(i))));
        }

        long mem2 = GC.GetTotalMemory(forceFullCollection: true);
        this.output.WriteLine($"Memory usage end: {mem}B");
        this.output.WriteLine($"Memory delta: {mem2 - mem}B");
    }

    [Theory(Skip = "performance test")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Create_StringCollection_ProducesMemoryEfficientObject(
            int cardinality)
    {
        long mem = GC.GetTotalMemory(forceFullCollection: true);

        ZipCollectionsFactory factory = new(Level);

        this.output.WriteLine("String collection test");
        this.output.WriteLine($"Level: {factory.Level}");
        this.output.WriteLine($"Objects: {N}, of length: {valueFactory(0).Length}");
        this.output.WriteLine($"Cardinality: {cardinality}");
        this.output.WriteLine($"Memory usage beginning: {mem}B");

        object[] stash = new object[N];

        for (int i = 0; i < N; i++)
        {
            stash[i] = factory.CreateCompressed(
                    Enumerable
                        .Range(0, cardinality)
                        .Select(i => valueFactory(i)));
        }

        long mem2 = GC.GetTotalMemory(forceFullCollection: true);
        this.output.WriteLine($"Memory usage end: {mem}B");
        this.output.WriteLine($"Memory delta: {mem2 - mem}B");
    }
#pragma warning restore xUnit1004 // Test methods should not be skipped
}

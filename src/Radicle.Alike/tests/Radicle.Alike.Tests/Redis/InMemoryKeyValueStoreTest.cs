namespace Radicle.Alike.Redis;

using System;
using System.Threading.Tasks;
using Xunit;

public class InMemoryKeyValueStoreTest
{
    [Fact]
    public void InMemoryKeyValueStore_NoRandomKeySampling_DoesNotEvict()
    {
        InMemoryKeyValueStore<int, bool> map = new(0);

        map.Set(1, true);
        map.Set(2, false, expiration: TimeSpan.FromTicks(1));

        int tries = 0;

        while (tries++ < 10)
        {
            _ = map.TryGetValue(1, out _);

            Assert.Equal(2UL, map.ContemporaryLength);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void InMemoryKeyValueStore_RandomKeySampling_DoesEvict(byte samples)
    {
        InMemoryKeyValueStore<int, bool> map = new(ttlEvictionSamples: samples);

        map.Set(1, true);
        map.Set(2, false, expiration: TimeSpan.FromTicks(1));

        Assert.Equal(2UL, map.ContemporaryLength);

        int tries = 0;

        while (tries++ < 10)
        {
            _ = map.TryGetValue(1, out _);

            if (map.ContemporaryLength < 2uL)
            {
                break;
            }
        }

        Assert.Equal(1UL, map.ContemporaryLength);
    }

    [Fact]
    public void ContemporaryLength_IncludesExpiredKeys()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);
        map.Set(4, false, expiration: TimeSpan.FromTicks(1));

        Assert.Equal(2UL, map.ContemporaryLength);

        Assert.True(map.Contains(1));
        Assert.False(map.Contains(4));

        Assert.Equal(1UL, map.ContemporaryLength);
    }

    [Fact]
    public void Contains_NullInput_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() => map.Contains(null!));
    }

    [Fact]
    public void Contains_ValidExistingKey_ReturnsTrue()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);

        Assert.True(map.Contains(1));
    }

    [Fact]
    public void Contains_ValidExistingKeyWithFutureExpiration_ReturnsTrue()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true, TimeSpan.FromSeconds(10));

        Assert.True(map.Contains(1));
    }

    [Fact]
    public void Contains_NonExistingKey_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);

        Assert.False(map.Contains(2));
    }

    [Fact]
    public async Task Contains_ValidExpiredKey_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true, TimeSpan.FromTicks(1));

        await Task.Delay(TimeSpan.FromMilliseconds(1));

        Assert.False(map.Contains(1));
    }

    [Fact]
    public void TryGetValue_NullInput_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() => map.TryGetValue(null!, out _));
    }

    [Fact]
    public void TryGetValue_ValidKey_ReturnsTrue()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);

        Assert.True(map.TryGetValue(1, out bool stored));
        Assert.True(stored);
    }

    [Fact]
    public void TryGetValue_ValidKeyWithFutureExpiration_ReturnsTrue()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true, TimeSpan.FromSeconds(10));

        Assert.True(map.TryGetValue(1, out bool stored));
        Assert.True(stored);
    }

    [Fact]
    public void TryGetValue_NonExistingKey_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);

        Assert.False(map.TryGetValue(2, out _));
    }

    [Fact]
    public async Task TryGetValue_ExpiredKey_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true, TimeSpan.FromTicks(1));

        await Task.Delay(TimeSpan.FromMilliseconds(1));

        Assert.False(map.TryGetValue(1, out _));
    }

    [Fact]
    public void GetOrAdd_NullKey_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() =>
                map.GetOrAdd(null!, _ => false));
    }

    [Fact]
    public void GetOrAdd_NullFactory_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() =>
                map.GetOrAdd("foo", null!));
    }

    [Fact]
    public void GetOrAdd_NullFactoredValue_Throws()
    {
        InMemoryKeyValueStore<string, object> map = new();

        ArgumentNullException exc = Assert.Throws<ArgumentNullException>(() =>
                map.GetOrAdd("foo", _ => null!));
        Assert.Contains("Value factory produced null value", exc.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetOrAdd_InvalidExpiration_Throws(int seconds)
    {
        InMemoryKeyValueStore<int, bool> map = new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                map.GetOrAdd(1, _ => true, TimeSpan.FromSeconds(seconds)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetOrAdd_ExistingValue_IsNotOverridden(bool expected)
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, expected);

        Assert.Equal(expected, map.GetOrAdd(1, _ => !expected));

        Assert.True(map.TryGetValue(1, out bool stored));
        Assert.Equal(expected, stored);
    }

    [Theory]
    [InlineData(1, 2)]
    public void GetOrAdd_NonExistingValue_IsOverridden(int oldValue, int newValue)
    {
        InMemoryKeyValueStore<int, int> map = new();

        map.Set(1, oldValue);

        Assert.Equal(newValue, map.GetOrAdd(2, _ => newValue));

        Assert.True(map.TryGetValue(2, out int stored));
        Assert.Equal(newValue, stored);
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task GetOrAdd_ExpiredValue_IsOverridden(int oldValue, int newValue)
    {
        InMemoryKeyValueStore<int, int> map = new();

        map.Set(1, oldValue, TimeSpan.FromTicks(1));

        await Task.Delay(TimeSpan.FromMilliseconds(1));

        Assert.Equal(newValue, map.GetOrAdd(1, _ => newValue));

        Assert.True(map.TryGetValue(1, out int stored));
        Assert.Equal(newValue, stored);
    }

    [Fact]
    public void Set_NullKey_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() => map.Set(null!, true));
    }

    [Fact]
    public void Set_NullValue_Throws()
    {
        InMemoryKeyValueStore<string, object> map = new();

        Assert.Throws<ArgumentNullException>(() => map.Set("foo", null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Set_InvalidExpiration_Throws(int seconds)
    {
        InMemoryKeyValueStore<int, bool> map = new();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
                map.Set(1, true, TimeSpan.FromSeconds(seconds)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Set_ExistingValue_IsOverridden(bool expected)
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, !expected);

        map.Set(1, expected);

        Assert.True(map.TryGetValue(1, out bool stored));
        Assert.Equal(expected, stored);
    }

    [Theory]
    [InlineData(1, 2)]
    public void Set_NonExistingValue_IsOverridden(int oldValue, int newValue)
    {
        InMemoryKeyValueStore<int, int> map = new();

        map.Set(1, oldValue);

        map.Set(2, newValue);

        Assert.True(map.TryGetValue(2, out int stored));
        Assert.Equal(newValue, stored);
    }

    [Theory]
    [InlineData(1, 2)]
    public async Task Set_ExpiredValue_IsOverridden(int oldValue, int newValue)
    {
        InMemoryKeyValueStore<int, int> map = new();

        map.Set(1, oldValue, TimeSpan.FromTicks(1));

        await Task.Delay(TimeSpan.FromMilliseconds(1));

        map.Set(1, newValue);

        Assert.True(map.TryGetValue(1, out int stored));
        Assert.Equal(newValue, stored);
    }

    [Fact]
    public void TryRemove_NullInput_Throws()
    {
        InMemoryKeyValueStore<string, bool> map = new();

        Assert.Throws<ArgumentNullException>(() => map.TryRemove(null!, out _));
    }

    [Fact]
    public void TryRemove_ExistingValue_ReturnsTrue()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true);
        Assert.Equal(1uL, map.ContemporaryLength);

        Assert.True(map.TryRemove(1, out bool stored));
        Assert.True(stored);
        Assert.Equal(0uL, map.ContemporaryLength);
    }

    [Fact]
    public async Task TryRemove_ExpiredValue_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        map.Set(1, true, TimeSpan.FromTicks(1));
        Assert.Equal(1uL, map.ContemporaryLength);

        await Task.Delay(TimeSpan.FromMilliseconds(1));

        Assert.False(map.TryRemove(1, out _));
        Assert.Equal(0uL, map.ContemporaryLength);
    }

    [Fact]
    public void TryRemove_NonExistingValue_ReturnsFalse()
    {
        InMemoryKeyValueStore<int, bool> map = new();

        Assert.Equal(0uL, map.ContemporaryLength);

        map.Set(1, true);

        Assert.Equal(1uL, map.ContemporaryLength);

        Assert.False(map.TryRemove(2, out _));
        Assert.Equal(1uL, map.ContemporaryLength);
    }
}

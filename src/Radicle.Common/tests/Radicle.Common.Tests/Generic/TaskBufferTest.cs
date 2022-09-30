namespace Radicle.Common.Generic;

using System.Threading.Tasks;
using Xunit;

public class TaskBufferTest
{
    [Fact]
    public async Task FlushAsync_TasksWithMetaData_ReturnsCorrectMetadata()
    {
        TaskBuffer<string, int> buffer = new(capacity: 12);

        for (int i = 0; i < 10; i++)
        {
            buffer.Enqueue(Task.FromResult($"{i}"), i);
        }

        await foreach ((string result, int meta) in buffer.FlushAsync())
        {
            Assert.Equal($"{meta}", result);
        }
    }
}

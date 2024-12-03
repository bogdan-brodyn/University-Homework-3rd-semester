// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPoolTests;

using MyThreadPool;

[TestFixture(1)]
[TestFixture(3)]
[TestFixture(5)]
public class ThreadsContainedCountTest(int threadsCount)
{
    private readonly ManualResetEvent manualResetEvent = new (false);
    private readonly int threadsCount = threadsCount;
    private volatile int actualThreadsCount = 0;

    [Test]
    [CancelAfter(2000)]
    public void Test(CancellationToken token)
    {
        // Arrange
        var threadPool = new MyThreadPool(this.threadsCount);

        // Act
        for (int i = 0; i < this.threadsCount; ++i)
        {
            threadPool.Submit<int>(this.Work);
        }

        while (this.actualThreadsCount != this.threadsCount)
        {
            if (token.IsCancellationRequested)
            {
                this.manualResetEvent.Set();
                threadPool.Shutdown();
                Assert.Fail();
            }
        }

        this.manualResetEvent.Set();
        threadPool.Shutdown();
        Assert.Pass();
    }

    private int Work()
    {
        Interlocked.Increment(ref this.actualThreadsCount);
        this.manualResetEvent.WaitOne();
        return 0;
    }
}

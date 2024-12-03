// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPoolTests;

using MyThreadPool;

public class InteractionTests
{
    private readonly int threadPoolThreadsCount = Environment.ProcessorCount;

    [Test]
    public void ThrowsException_WhenSubmitingOrContinuingTaskAfterShutdown_Test()
    {
        // Arrange
        var threadPool = new MyThreadPool(this.threadPoolThreadsCount);

        // Act
        var taskSubmited = threadPool.Submit<int>(() => 0);
        threadPool.Shutdown();

        // Assert
        Assert.Throws<OperationCanceledException>(() => threadPool.Submit<int>(() => 0));
        Assert.Throws<OperationCanceledException>(() => taskSubmited.ContinueWith<int>((input) => input));
    }

    [TestCase(1000)]
    public void SinglethreadedTest(int startsCount)
    {
        var threadPool = new MyThreadPool(this.threadPoolThreadsCount);
        BasicInteractionTest(threadPool, startsCount);
        threadPool.Shutdown();
    }

    [TestCase(5, 1000)]
    [TestCase(10, 1000)]
    public void MultithreadedTest(int userSpaceThreadsCount, int startsCount)
    {
        var threadPool = new MyThreadPool(this.threadPoolThreadsCount);
        var threads = new Thread[userSpaceThreadsCount];

        for (int i = 0; i < userSpaceThreadsCount; ++i)
        {
            threads[i] = new Thread(
                () => BasicInteractionTest(threadPool, startsCount));
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        threadPool.Shutdown();
    }

    private static void BasicInteractionTest(MyThreadPool threadPool, int startsCount)
    {
        static int Work(ref int a, int b)
        {
            return a += b;
        }

        // Arrange
        var submitedTasks = new IMyTask<int>[startsCount];
        var continuationTasks = new IMyTask<int>[startsCount];
        var unsubmitedTasks = new IMyTask<int>[startsCount];

        // Act
        for (var i = 0; i < startsCount; ++i)
        {
            var variableClosured = i;
            submitedTasks[i] = threadPool.Submit<int>(() => Work(ref variableClosured, 1));
            continuationTasks[i] = submitedTasks[i].ContinueWith<int>((input) => Work(ref variableClosured, input));
        }

        // Assert
        for (var i = 0; i < startsCount; ++i)
        {
            Assert.That(submitedTasks[i].Result, Is.EqualTo(i + 1));
            Assert.That(continuationTasks[i].Result, Is.EqualTo(2 * (i + 1)));
        }
    }
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace LazyTests.TestCases;

using System.Diagnostics;

public class TestCaseWithLongVariableClosureAndThreadSleep : ITestCase<long>
{
    private readonly Stopwatch stopwatch = new ();

    private long variableToBeCaptured = 0;

    public long ExpectedGetResult => 0;

    public Func<long> Supplier => () =>
    {
        var tmp = this.variableToBeCaptured;
        this.stopwatch.Start();
        Thread.Sleep(100);
        this.stopwatch.Stop();
        this.variableToBeCaptured = this.stopwatch.ElapsedTicks;
        return tmp;
    };
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1200 // Using directives should be placed correctly
using BenchmarkDotNet.Running;
#pragma warning restore SA1200 // Using directives should be placed correctly

BenchmarkRunner.Run<ConsoleAppBenchmark.Benchmark>();

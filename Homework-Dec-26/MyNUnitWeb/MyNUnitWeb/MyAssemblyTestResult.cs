// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb;

public class MyAssemblyTestResult(string assemblyName, string testResult)
{
    public string AssemblyName { get; set; } = assemblyName;

    public string TestResult { get; set; } = testResult;
}

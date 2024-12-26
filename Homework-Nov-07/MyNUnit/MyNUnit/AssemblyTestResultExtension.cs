// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Implements functionality for testing an assembly.
/// </summary>
public static class AssemblyTestResultExtension
{
    /// <summary>
    /// Serializes <see cref="AssemblyTestResult"/> instanse.
    /// </summary>
    /// <param name="assemblyTestResult">The instanse to be serialized.</param>
    /// <returns>Serialized value.</returns>
    public static string Serialize(this MyNUnit.AssemblyTestResult assemblyTestResult)
    {
        var offset = string.Empty;
        var stringWriter = new StringWriter();

        if (assemblyTestResult.TypesTestResult.Count == 0)
        {
            stringWriter.WriteLine($"{offset}{assemblyTestResult.AssemblyName} assembly doesn't contain any tests");
            return stringWriter.ToString();
        }

        stringWriter.WriteLine($"{offset}{assemblyTestResult.AssemblyName}");
        offset += "    ";
        foreach (var typeTestResult in assemblyTestResult.TypesTestResult)
        {
            if (typeTestResult.IsInvalid)
            {
                stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} can't be tested, since it doesn't follows the format");
                continue;
            }

            if (typeTestResult.IsErrored)
            {
                stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} testing failed since an unexpected error occured while calling [BeforeClass] or [AfterClass] method");
                continue;
            }

            if (typeTestResult.TestResults.Count == 0)
            {
                continue;
            }

            stringWriter.WriteLine($"{offset}{typeTestResult.TypeName}");
            offset += "    ";
            foreach (var testResult in typeTestResult.TestResults)
            {
                stringWriter.WriteLine($"{offset}{testResult.TestName}: {testResult.Result}");
            }

            offset = offset[^4..];
        }

        return stringWriter.ToString();
    }
}

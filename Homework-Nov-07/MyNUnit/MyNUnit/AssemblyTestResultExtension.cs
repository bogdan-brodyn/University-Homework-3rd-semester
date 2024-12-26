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
    public static string Serialize(this AssemblyTestResult assemblyTestResult)
    {
        var offset = string.Empty;
        var hasTestResults = false;
        var stringWriter = new StringWriter();

        stringWriter.WriteLine($"{offset}{assemblyTestResult.AssemblyName}");
        offset += "    ";
        foreach (var typeTestResult in assemblyTestResult.TypesTestResult)
        {
            switch (typeTestResult.State)
            {
                case TypeTestResultSpecialValue.InvalidProgram:
                    hasTestResults = true;
                    stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} can't be tested, since it doesn't follows the format");
                    continue;
                case TypeTestResultSpecialValue.KnownError:
                    hasTestResults = true;
                    stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} testing failed since an unexpected error occured while calling [BeforeClass] or [AfterClass] method");
                    continue;
                case TypeTestResultSpecialValue.SomeFilesNotFound:
                    hasTestResults = true;
                    stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} testing failed since not all files were found");
                    continue;
                case TypeTestResultSpecialValue.UnknownError:
                    hasTestResults = true;
                    stringWriter.WriteLine($"{offset}{typeTestResult.TypeName} testing failed while thrown unknown error");
                    continue;
            }

            if (typeTestResult.TestResults.Count != 0)
            {
                hasTestResults = true;
                stringWriter.WriteLine($"{offset}{typeTestResult.TypeName}");
                offset += "    ";
                foreach (var testResult in typeTestResult.TestResults)
                {
                    stringWriter.WriteLine($"{offset}{testResult.TestName}: {testResult.Result}");
                }

                offset = offset[^4..];
            }
        }

        if (!hasTestResults)
        {
            stringWriter.WriteLine($"{offset}The assembly doesn't contain any tests");
            return stringWriter.ToString();
        }

        return stringWriter.ToString();
    }
}

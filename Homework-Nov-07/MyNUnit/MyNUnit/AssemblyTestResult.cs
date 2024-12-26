// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record AssemblyTestResult(string AssemblyName, List<TypeTestResult> TypesTestResult);

public record TypeTestResult(string TypeName, List<TestResult> TestResults, TypeTestResultSpecialValue State = TypeTestResultSpecialValue.Default);

public record TestResult(string TestName, string Result);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

#pragma warning disable SA1600 // Elements should be documented
public enum TypeTestResultSpecialValue
#pragma warning restore SA1600 // Elements should be documented
{
#pragma warning disable SA1602 // Enumeration items should be documented
    Default,
    KnownError,
    UnknownError,
    InvalidProgram,
    SomeFilesNotFound,
#pragma warning restore SA1602 // Enumeration items should be documented
}

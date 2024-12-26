// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitTests;

using System.Reflection;
using static MyNUnit.MyNUnit;
using static MyNUnit.AssemblyTestResultExtension;

public class MyNUnitTest
{
    [Test]
    public async Task Test()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(@"TestClasslib.dll");

        // Act
        var assemblyTestResult = await TestAssembly(assembly);

        // Assert
        var assemblyTestResultSerialized = assemblyTestResult.Serialize();
        var expectedResult = File.ReadAllText(@"../../../ExpectedResult.txt");
        Assert.That(actual: assemblyTestResultSerialized, Is.EqualTo(expectedResult));
    }
}

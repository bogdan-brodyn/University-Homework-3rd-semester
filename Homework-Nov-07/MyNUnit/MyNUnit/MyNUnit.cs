// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Implements functionality for testing an assembly.
/// </summary>
public static class MyNUnit
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record TestResult(string TestName, string Result);

    public record TypeTestResult(string TypeName, List<TestResult> TestResults, bool IsCorrect = true);

    public record AssemblyTestResult(string AssemblyName, List<TypeTestResult> TypeTestResults);

    private record Test(string Name, Func<bool> Method, Type? Expected, string? Ignore);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Tests the assembly loaded from the path.
    /// </summary>
    /// <param name="assemblyPath">The assembly to be tested location.</param>
    /// <returns>The assembly testing result.</returns>
    public static async Task<AssemblyTestResult> TestAssembly(string assemblyPath)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        return await TestAssembly(assembly);
    }

    /// <summary>
    /// Tests the assembly given.
    /// </summary>
    /// <param name="assembly">The assembly to be tested.</param>
    /// <returns>The assembly testing result.</returns>
    public static async Task<AssemblyTestResult> TestAssembly(Assembly assembly)
    {
        var assemblyTestResult = new AssemblyTestResult(assembly.GetName().FullName, new List<TypeTestResult>());

        var typesTestResult = new List<(string TypeName, Task<TypeTestResult> TestTypeTask)>();
        foreach (var exportedType in assembly.ExportedTypes)
        {
            var testTypeTask = Task.Run(() => TestType(exportedType));
            typesTestResult.Add((
                TypeName: exportedType.Name,
                TestTypeTask: testTypeTask));
        }

        foreach (var (typeName, testTypeTask) in typesTestResult)
        {
            try
            {
                var typeTestResult = await testTypeTask;
                assemblyTestResult.TypeTestResults.Add(typeTestResult);
            }
            catch
            {
                assemblyTestResult.TypeTestResults.Add(new TypeTestResult(typeName, new (), false));
            }
        }

        return assemblyTestResult;
    }

    private static TypeTestResult TestType(Type type)
    {
        var typeTestResult = new TypeTestResult(type.Name, new List<TestResult>());
        var (beforeClass, afterClass) = GetBeforeClassAndAfterClass(type);
        var (before, after, tests) = GetBeforeAndAfterAndTests(type);

        beforeClass?.Invoke();
        foreach (var test in tests)
        {
            if (test.Ignore is string ignoreMessage)
            {
                typeTestResult.TestResults.Add(new TestResult(
                    TestName: test.Name,
                    Result: $"Test was ignored with message: {ignoreMessage}"));
                continue;
            }

            before?.Invoke();
            TestResult testResult;
            try
            {
                var isTestPassed = test.Method.Invoke();
                testResult = new TestResult(
                    TestName: test.Name,
                    Result: isTestPassed ? "passed" : "failed");
            }
            catch (TargetInvocationException targetInvocationException)
            {
                var result =
                    targetInvocationException.InnerException?.GetType() == test.Expected
                    && test.Expected is not null
                        ? "test passed since the exception was expected"
                        : $"test failed with exception {targetInvocationException.GetType()} but {test.Expected} was expected";
                testResult = new TestResult(
                    TestName: test.Name,
                    Result: result);
            }

            after?.Invoke();
            typeTestResult.TestResults.Add(testResult);
        }

        afterClass?.Invoke();
        return typeTestResult;
    }

    private static (Action? beforeClass, Action? afterClass) GetBeforeClassAndAfterClass(Type type)
    {
        Action? beforeClass = null;
        Action? afterClass = null;

        foreach (var methodInfo in type.GetMethods(
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static))
        {
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(BeforeClassAttribute), action: ref beforeClass);
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(AfterClassAttribute), action: ref afterClass);
        }

        return (beforeClass, afterClass);
    }

    private static (Action? before, Action? after, List<Test> tests) GetBeforeAndAfterAndTests(Type type)
    {
        Action? before = null;
        Action? after = null;
        var tests = new List<Test>();
        var obj = Activator.CreateInstance(type);

        foreach (var methodInfo in type.GetMethods(
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
        {
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(BeforeAttribute), action: ref before, obj);
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(AfterAttribute), action: ref after, obj);

            if (methodInfo.GetCustomAttribute(typeof(TestAttribute)) is TestAttribute testAttribute)
            {
                if (methodInfo.GetParameters().Length != 0)
                {
                    throw new InvalidProgramException(
                        $"The test method '{methodInfo.Name}' must not require any parameters");
                }

                if (methodInfo.ReturnParameter.ParameterType != typeof(bool))
                {
                    throw new InvalidProgramException(
                        $"The test method '{methodInfo.Name}' must return boolean type value");
                }

                tests.Add(new Test(
                    Name: methodInfo.Name,
                    Method: () => (bool)(methodInfo.Invoke(obj: obj, parameters: null) ?? throw new InvalidProgramException()),
                    Expected: testAttribute.Expected,
                    Ignore: testAttribute.Ignore));
            }
        }

        return (before, after, tests);
    }

    private static void ExtendActionWithMethodIfSuitable(
        MethodInfo methodInfo, Type attributeType, ref Action? action, object? obj = null)
    {
        if (methodInfo.GetCustomAttribute(attributeType) is null)
        {
            return;
        }

        if (methodInfo.GetParameters().Length != 0)
        {
            throw new InvalidProgramException(
                $"The method '{methodInfo.Name}' that has an attribute of MyNUnit must not require any parameters");
        }

        action += () => methodInfo.Invoke(obj: obj, parameters: null);
    }
}

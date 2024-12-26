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
    private record Test(string Name, Func<object, bool> Method, Type? Expected, string? Ignore);
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
        var assemblyTestResult = new AssemblyTestResult(
            AssemblyName: assembly.GetName().FullName,
            TypesTestResult: new ());

        var typesTestResult = new List<(string TypeName, Task<TypeTestResult> TypeTestTask)>();
        foreach (var exportedType in assembly.ExportedTypes)
        {
            var testTypeTask = Task.Run(() => TestType(exportedType));
            typesTestResult.Add((
                TypeName: exportedType.Name,
                TypeTestTask: testTypeTask));
        }

        foreach (var (typeName, typeTestTask) in typesTestResult)
        {
            try
            {
                var typeTestResult = await typeTestTask;
                assemblyTestResult.TypesTestResult.Add(typeTestResult);
            }
            catch (FileNotFoundException)
            {
                assemblyTestResult.TypesTestResult.Add(new TypeTestResult(
                    TypeName: typeName,
                    TestResults: new (),
                    State: TypeTestResultSpecialValue.SomeFilesNotFound));
            }
            catch (TargetInvocationException)
            {
                assemblyTestResult.TypesTestResult.Add(new TypeTestResult(
                    TypeName: typeName,
                    TestResults: new (),
                    State: TypeTestResultSpecialValue.InvalidProgram));
            }
            catch
            {
                assemblyTestResult.TypesTestResult.Add(new TypeTestResult(
                    TypeName: typeName,
                    TestResults: new (),
                    State: TypeTestResultSpecialValue.UnknownError));
            }
        }

        return assemblyTestResult;
    }

    private static TypeTestResult TestType(Type type)
    {
        var typeTestResult = new TypeTestResult(TypeName: type.Name, TestResults: new List<TestResult>());
        var (beforeClass, afterClass) = GetBeforeClassAndAfterClass(type);
        var (before, after, tests) = GetBeforeAndAfterAndTests(type);

        try
        {
            beforeClass?.Invoke();
        }
        catch
        {
            return new TypeTestResult(TypeName: type.Name, TestResults: new (), State: TypeTestResultSpecialValue.KnownError);
        }

        foreach (var test in tests)
        {
            if (test.Ignore is string ignoreMessage)
            {
                typeTestResult.TestResults.Add(new TestResult(
                    TestName: test.Name,
                    Result: $"Test was ignored with message: '{ignoreMessage}'"));
                continue;
            }

            var obj = Activator.CreateInstance(type) ?? throw new InvalidProgramException();

            try
            {
                before?.Invoke(obj);
            }
            catch
            {
                typeTestResult.TestResults.Add(new TestResult(
                    TestName: test.Name,
                    Result: "test failed since an unexpected error occured while calling [Before] method"));
                continue;
            }

            TestResult testResult;
            try
            {
                var isTestPassed = test.Method.Invoke(obj);
                var result = isTestPassed && test.Expected is null ? "passed" : "failed";
                testResult = new TestResult(TestName: test.Name, Result: result);
            }
            catch (TargetInvocationException targetInvocationException)
            {
                var result =
                    targetInvocationException.InnerException?.GetType() == test.Expected
                    && test.Expected is not null
                        ? "passed"
                        : $"test failed with exception {targetInvocationException.GetType()} but {test.Expected} was expected";
                testResult = new TestResult(TestName: test.Name, Result: result);
            }

            try
            {
                after?.Invoke(obj);
                typeTestResult.TestResults.Add(testResult);
            }
            catch
            {
                typeTestResult.TestResults.Add(new TestResult(
                    TestName: test.Name,
                    Result: "test failed since an unexpected error occured while calling [After] method"));
            }
        }

        try
        {
            afterClass?.Invoke();
            return typeTestResult;
        }
        catch
        {
            return new TypeTestResult(TypeName: type.Name, TestResults: new (), State: TypeTestResultSpecialValue.KnownError);
        }
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

    private static (Action<object>? before, Action<object>? after, List<Test> tests) GetBeforeAndAfterAndTests(Type type)
    {
        Action<object>? before = null;
        Action<object>? after = null;
        var tests = new List<Test>();

        foreach (var methodInfo in type.GetMethods(
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
        {
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(BeforeAttribute), action: ref before);
            ExtendActionWithMethodIfSuitable(
                methodInfo: methodInfo, attributeType: typeof(AfterAttribute), action: ref after);

            if (methodInfo.GetCustomAttribute(typeof(TestAttribute)) is TestAttribute testAttribute)
            {
                ThrowIfMethodNeedParameters(methodInfo);
                if (methodInfo.ReturnParameter.ParameterType != typeof(bool))
                {
                    throw new InvalidProgramException(
                        $"The test method '{methodInfo.Name}' must return boolean type value");
                }

                tests.Add(new Test(
                    Name: methodInfo.Name,
                    Method: (obj) => (bool)(methodInfo.Invoke(obj: obj, parameters: null) ?? throw new InvalidProgramException()),
                    Expected: testAttribute.Expected,
                    Ignore: testAttribute.Ignore));
            }
        }

        return (before, after, tests);
    }

    private static void ExtendActionWithMethodIfSuitable(
        MethodInfo methodInfo, Type attributeType, ref Action? action)
    {
        if (methodInfo.GetCustomAttribute(attributeType) is not null)
        {
            ThrowIfMethodNeedParameters(methodInfo);
            action += () => methodInfo.Invoke(obj: null, parameters: null);
        }
    }

    private static void ExtendActionWithMethodIfSuitable(
        MethodInfo methodInfo, Type attributeType, ref Action<object>? action)
    {
        if (methodInfo.GetCustomAttribute(attributeType) is not null)
        {
            ThrowIfMethodNeedParameters(methodInfo);
            action += (obj) => methodInfo.Invoke(obj: obj, parameters: null);
        }
    }

    private static void ThrowIfMethodNeedParameters(MethodInfo methodInfo)
    {
        if (methodInfo.GetParameters().Length != 0)
        {
            throw new InvalidProgramException(
                $"The method '{methodInfo.Name}' that has an attribute of MyNUnit must not require any parameters");
        }
    }
}

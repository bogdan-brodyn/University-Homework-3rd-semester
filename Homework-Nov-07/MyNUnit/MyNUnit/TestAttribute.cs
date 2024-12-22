// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Represents the attribute for test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    public TestAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// Is used when an exception is expected to be thrown.
    /// </summary>
    /// <param name="expected">The expected exception type.</param>
    public TestAttribute(Type expected)
    {
        this.Expected = expected;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// Is used when the test is to be ignored.
    /// </summary>
    /// <param name="ignore">The ignore message.</param>
    public TestAttribute(string ignore)
    {
        this.Ignore = ignore;
    }

    /// <summary>
    /// Gets the exception type if exception is expected, otherwise null.
    /// </summary>
    public Type? Expected { get; }

    /// <summary>
    /// Gets the test ignore message if test must be ignored, otherwise null.
    /// </summary>
    public string? Ignore { get; }
}

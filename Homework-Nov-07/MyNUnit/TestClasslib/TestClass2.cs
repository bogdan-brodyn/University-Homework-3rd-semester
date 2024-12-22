// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace TestClasslib;

using MyNUnit;

public class TestClass2
{
    [Test(expected: typeof(InvalidOperationException))]
    public bool TestToBePassed1()
    {
        throw new InvalidOperationException();
    }

    [Test(expected: typeof(InvalidOperationException))]
    public bool TestToBeFailed1()
    {
        throw new InvalidDataException();
    }

    [Test(ignore: "This test is to be ignored")]
    public bool TestToBeIgnored1()
    {
        throw new InvalidDataException();
    }
}

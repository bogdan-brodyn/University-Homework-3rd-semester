// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace TestClasslib;

using MyNUnit;

public class TestClass1
{
    private static int counter = 0;

    [BeforeClass]
    public static void BeforeClass1()
    {
        ++counter;
    }

    [Before]
    public void Before1()
    {
        ++counter;
    }

    [Test]
    public bool TestToBePassed1()
    {
        return counter++ == 2;
    }

    [Test]
    public bool TestToBePassed2()
    {
        return counter++ == 3;
    }

    [Test]
    public bool TestToBeFailed1()
    {
        return counter == 0;
    }

    [After]
    public void After1()
    {
        --counter;
    }

    [AfterClass]
#pragma warning disable SA1204 // Static elements should appear before instance elements
    public static void AfterClass1()
    {
        if (counter != 3)
        {
            throw new Exception();
        }
    }
#pragma warning restore SA1204 // Static elements should appear before instance elements
}

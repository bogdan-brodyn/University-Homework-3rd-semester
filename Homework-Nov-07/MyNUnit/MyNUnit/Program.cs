// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

if (args.Length != 1)
{
    throw new InvalidDataException("There is to be one argument");
}

var assemblyTestResult = await MyNUnit.MyNUnit.TestAssembly(args[0]);

var assemblyTestResultSerialized = MyNUnit.AssemblyTestResultExtension.Serialize(assemblyTestResult);

Console.WriteLine("Testing result:");
Console.WriteLine();
Console.WriteLine(assemblyTestResultSerialized);

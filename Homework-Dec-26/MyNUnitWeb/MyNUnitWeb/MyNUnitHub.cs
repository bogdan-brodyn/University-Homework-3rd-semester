// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb;

using MyNUnit;

using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

public class MyNUnitHub : Hub
{
    private static int tmpFolderNumber = 0;

    public async Task TestAssemblies(List<MyAssembly> assembliesSerialized)
    {
        var currentTmpFolderNumber = Interlocked.Increment(ref tmpFolderNumber);
        var tmpDirectory = Directory.CreateDirectory(@$"./bin/Debug/net9.0/Tmp/TmpDirectory{currentTmpFolderNumber}");
        foreach (var myAssembly in assembliesSerialized)
        {
            const string pattern = ";base64,";
            var contentStartIndex = myAssembly.File.IndexOf(pattern) + pattern.Length;
            var content = myAssembly.File[contentStartIndex..];
            var bytes = Convert.FromBase64String(content);
            File.WriteAllBytes(Path.Combine(tmpDirectory.FullName, myAssembly.Name), bytes);
        }

        var assemblyLoadContext = new AssemblyLoadContext(name: tmpDirectory.Name, isCollectible: true);
        var assemblies = new List<(string assemblyName, Assembly assembly)>();
        foreach (var file in tmpDirectory.GetFiles())
        {
            assemblies.Add((file.Name, assemblyLoadContext.LoadFromAssemblyPath(file.FullName)));
        }

        var assembliesTestResult = new List<MyAssemblyTestResult>();
        foreach (var (assemblyName, assembly) in assemblies)
        {
            var assemblyTestResult = await MyNUnit.TestAssembly(assembly);
            assembliesTestResult.Add(new MyAssemblyTestResult(assemblyName, assemblyTestResult.Serialize()));
        }

        assemblyLoadContext.Unload();

        try
        {
            Directory.Delete(tmpDirectory.FullName, true);
        }
        catch
        {
            Console.Error.WriteLine("Couldn't delete temp directory, it must be done manualy");
        }

        var assembliesTestResultSerialized = JsonSerializer.Serialize(assembliesTestResult);
        await this.Clients.Caller.SendAsync("ReceiveAssembliesTestResult", assembliesTestResultSerialized);
    }
}

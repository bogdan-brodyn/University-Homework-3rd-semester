// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Tests;

using System.Text;
using static Server.Server;

public class ServerTests
{
    private const string RequestFile = "TmpRequest.txt";
    private const string ResponseFile = "TmpResponse.txt";

    [TestCase("1 TestDir", "5 (TestSubdir1 true) (TestSubdir2 true) (TestFile1 false) (TestFile2 false) (TestFile3 false)")]
    [TestCase("1 ./TestDir", "5 (TestSubdir1 true) (TestSubdir2 true) (TestFile1 false) (TestFile2 false) (TestFile3 false)")]
    [TestCase(@"1 ./TestDir/TestSubdir1", "2 (TestSubsubdir1 true) (TestFile1 false)")]
    [TestCase(@"1 ./TestDir/TestSubdir2", "1 (TestFile1 false)")]
    [TestCase("1 ./TestDir/TestSubdir3", "-1")]
    public async Task TestProcessClient_WithListRequest(string request, string expectedResponse)
    {
        // Arrange
        File.WriteAllText(RequestFile, request);

        // Act
        using (var requestStream = File.Open(RequestFile, FileMode.Open, FileAccess.Read))
        using (var responseStream = File.Open(ResponseFile, FileMode.Create, FileAccess.Write))
        {
            await ProcessClient(requestStream, responseStream);
        }

        // Assert
        var actualResponse = File.ReadAllLines(ResponseFile);
        Assert.That(actualResponse, Has.Length.EqualTo(1));
        Assert.That(actualResponse[0], Is.EqualTo(expectedResponse));
    }

    [TestCase("2 TestDir", "-1")]
    [TestCase("2 ./TestDir/TestFile1", "0 ")]
    [TestCase(@"2 ./TestDir/TestFile2", "22 dfgh\\asf'a\\sf']']][[]]")]
    public async Task TestProcessClient_WithGetRequest(string request, string expectedResponse)
    {
        // Arrange
        File.WriteAllText(RequestFile, request);

        // Act
        using (var requestStream = File.Open(RequestFile, FileMode.Open, FileAccess.Read))
        using (var responseStream = File.Open(ResponseFile, FileMode.Create, FileAccess.Write))
        {
            await ProcessClient(requestStream, responseStream);
        }

        // Assert
        var actualResponse = File.ReadAllText(ResponseFile, Encoding.Latin1);
        Assert.That(actualResponse, Is.EqualTo(expectedResponse));
    }
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Tests;

using System.Text;
using static Client.Client;

public class ClientTests
{
    private const string RequestFile = "TmpRequest.txt";
    private const string ResponseFile = "TmpResponse.txt";

    [TestCase(".", "1 .", "1 (abrabra false)")]
    [TestCase("Test", "1 Test", "2 (abcabc true) (abccba false)")]
    [TestCase(@"Test\TestCase", @"1 Test\TestCase", "-1")]
    public async Task TestProcessListRequestMethod(string path, string expectedRequest, string expectedResponse)
    {
        // Arrange
        File.WriteAllText(ResponseFile, expectedResponse);

        // Act
        string actualResponse;
        using (var requestStream = File.Open(RequestFile, FileMode.Create, FileAccess.Write))
        using (var responseStream = File.Open(ResponseFile, FileMode.Open, FileAccess.Read))
        {
            actualResponse = await ProcessListRequest(path, requestStream, responseStream);
        }

        // Assert
        Assert.That(actualResponse, Is.EqualTo(expectedResponse));
        var actualRequest = File.ReadAllLines(RequestFile);
        Assert.That(actualRequest, Has.Length.EqualTo(1));
        Assert.That(actualRequest[0], Is.EqualTo(expectedRequest));
    }

    [TestCase("test.txt", "2 test.txt", "asdfghjk")]
    [TestCase(@"Test\test.txt", @"2 Test\test.txt", "ertyuidfvbnmfddf/.,...<><<>}{}{}[[;&]]")]
    public async Task TestProcessGetRequestMethod(string path, string expectedRequest, string expectedBytesEncoded)
    {
        // Arrange
        var expectedBytes = Encoding.Latin1.GetBytes(expectedBytesEncoded);
        File.WriteAllText(ResponseFile, $"{expectedBytes.LongLength} {expectedBytesEncoded}", Encoding.Latin1);

        // Act
        byte[] actualResponse;
        using (var requestStream = File.Open(RequestFile, FileMode.Create, FileAccess.Write))
        using (var responseStream = File.Open(ResponseFile, FileMode.Open, FileAccess.Read))
        {
            actualResponse = await ProcessGetRequest(path, requestStream, responseStream);
        }

        // Assert
        Assert.That(actualResponse, Is.EqualTo(expectedBytes));
        var actualRequest = File.ReadAllLines(RequestFile);
        Assert.That(actualRequest, Has.Length.EqualTo(1));
        Assert.That(actualRequest[0], Is.EqualTo(expectedRequest));
    }
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Client;

using System.Text;

/// <summary>
/// Implements the client interaction with the server.
/// </summary>
public static class Client
{
    /// <summary>
    /// Sends the list request to the server and returns its response.
    /// </summary>
    /// <param name="path">The path to the directory to be listed.</param>
    /// <param name="requestStream">The stream to which the request is sent.</param>
    /// <param name="responseStream">The stream from which the response is read.</param>
    /// <returns>The directory content.</returns>
    public static async Task<string> ProcessListRequest(string path, Stream requestStream, Stream responseStream)
    {
        ThrowIfWhiteSpaceContained(path);
        await SendRequest(requestStream, $"1 {path}");
        return await ProcessListResponse(responseStream);
    }

    /// <summary>
    /// Sends the get request to the server and returns its response.
    /// </summary>
    /// <param name="path">The path to the file to be got.</param>
    /// <param name="requestStream">The stream to which the request is sent.</param>
    /// <param name="responseStream">The stream from which the response is read.</param>
    /// <returns>The file content.</returns>
    public static async Task<byte[]> ProcessGetRequest(string path, Stream requestStream, Stream responseStream)
    {
        ThrowIfWhiteSpaceContained(path);
        await SendRequest(requestStream, $"2 {path}");
        return await ProcessGetResponse(responseStream);
    }

    private static void ThrowIfWhiteSpaceContained(string path)
    {
        if (path.Contains(' '))
        {
            throw new ArgumentException("The path must not contain white spaces", nameof(path));
        }
    }

    private static async Task SendRequest(Stream stream, string request)
    {
        var streamWriter = new StreamWriter(stream, Encoding.UTF8);
        await streamWriter.WriteLineAsync(request);
        await streamWriter.FlushAsync();
    }

    private static async Task<string> ProcessListResponse(Stream stream)
    {
        var streamReader = new StreamReader(stream, Encoding.UTF8);
        return await streamReader.ReadLineAsync() ?? throw new InvalidProgramException();
    }

    private static async Task<byte[]> ProcessGetResponse(Stream stream)
    {
        var streamReader = new StreamReader(stream, Encoding.Latin1);
        var response = await streamReader.ReadToEndAsync();
        Console.WriteLine(response);

        var firstSpaceIndex = response.IndexOf(' ');
        if (firstSpaceIndex == -1)
        {
            throw new InvalidDataException("The response is invalid, the file requested might not be contained");
        }

        var sizeString = response[..firstSpaceIndex];
        var contentString = response[(firstSpaceIndex + 1) ..];
        var size = long.Parse(sizeString);
        var content = Encoding.Latin1.GetBytes(contentString);
        if (content.Length != size)
        {
            throw new InvalidDataException("The response is invalid, since its content length doesn't match size it must have");
        }

        return content;
    }
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Server;

using System.Text;

/// <summary>
/// Implements the server interaction with the client.
/// </summary>
public static class Server
{
    /// <summary>
    /// Processes the client request and sends the response.
    /// </summary>
    /// <param name="requestStream">The stream from which the request is read.</param>
    /// <param name="responseStream">The stream to which the response is sent.</param>
    /// <returns>A task that represents the asynchronous client process operation.</returns>
    public static async Task ProcessClient(Stream requestStream, Stream responseStream)
    {
        var request = await ReceiveRequest(requestStream);
        await ProcessRequestAndSendResponse(responseStream, request);
    }

    private static async Task<string> ReceiveRequest(Stream stream)
    {
        var streamReader = new StreamReader(stream, Encoding.UTF8);
        var request = await streamReader.ReadLineAsync();

        if (request is null)
        {
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request must not be null");
            throw new InvalidDataException("The request must not be null");
        }

        var firstSpaceIndex = request.IndexOf(' ');
        var secondSpaceIndex = request.IndexOf(' ', firstSpaceIndex + 1);
        if ((request[0] != '1' && request[0] != '2') || firstSpaceIndex != 1 || secondSpaceIndex != -1)
        {
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request format must be followed");
            throw new InvalidDataException("The request format must be followed");
        }

        if (request.Contains(".."))
        {
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request path must not contain '..'");
            throw new InvalidDataException("The request path must not contain '..'");
        }

        return request;
    }

    private static async Task ProcessRequestAndSendResponse(Stream stream, string request)
    {
        if (request[0] == '1')
        {
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            var response = ProduceListResponse(request[2..]);
            await streamWriter.WriteLineAsync(response);
            await streamWriter.FlushAsync();
        }
        else if (request[0] == '2')
        {
            var streamWriter = new StreamWriter(stream, Encoding.Latin1);
            var response = await ProduceGetResponse(request[2..]);
            await streamWriter.WriteAsync(response);
            await streamWriter.FlushAsync();
        }
        else
        {
            throw new InvalidProgramException();
        }
    }

    private static string ProduceListResponse(string path)
    {
        if (!Directory.Exists(path))
        {
            return "-1";
        }

        var count = 0;
        var stringBuilder = new StringBuilder();

        foreach (var subdirectory in Directory.GetDirectories(path).Order())
        {
            stringBuilder.Append($" ({Path.GetFileName(subdirectory)} true)");
            ++count;
        }

        foreach (var file in Directory.GetFiles(path).Order())
        {
            stringBuilder.Append($" ({Path.GetFileName(file)} false)");
            ++count;
        }

        count = count != 0 ? count : -1;
        return $"{count}{stringBuilder}";
    }

    private static async Task<string> ProduceGetResponse(string path)
    {
        if (File.Exists(path))
        {
            var bytes = await File.ReadAllBytesAsync(path);
            var @string = Encoding.Latin1.GetString(bytes);
            return $"{bytes.LongLength} {@string}";
        }
        else
        {
            return $"-1";
        }
    }
}

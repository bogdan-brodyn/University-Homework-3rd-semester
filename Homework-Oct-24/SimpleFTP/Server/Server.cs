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
    /// <param name="networkStream">The network stream.</param>
    /// <returns>A task that represents the asynchronous client process operation.</returns>
    public static async Task ProcessClient(Stream networkStream)
    {
        var request = await ReceiveRequest(networkStream);
        await ProcessRequest(networkStream, request);
    }

    private static async Task<string> ReceiveRequest(Stream networkStream)
    {
        var streamReader = new StreamReader(networkStream, Encoding.UTF8);
        var request = await streamReader.ReadLineAsync();

        if (request is null)
        {
            using var streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request must not be null");
            throw new InvalidDataException("The request must not be null");
        }

        var firstSpaceIndex = request.IndexOf(' ');
        var secondSpaceIndex = request.IndexOf(' ', firstSpaceIndex + 1);
        if ((request[0] != '1' && request[0] != '2') || firstSpaceIndex != 1 || secondSpaceIndex != -1)
        {
            using var streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request format must be followed");
            throw new InvalidDataException("The request format must be followed");
        }

        if (request.Contains(".."))
        {
            using var streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
            await streamWriter.WriteLineAsync("The request path must not contain '..'");
            throw new InvalidDataException("The request path must not contain '..'");
        }

        return request;
    }

    private static async Task ProcessRequest(Stream networkStream, string request)
    {
        if (request[0] == '1')
        {
            var streamWriter = new StreamWriter(networkStream, Encoding.UTF8);
            var response = ProduceListResponse(request[2..]);
            await streamWriter.WriteLineAsync(response);
            await streamWriter.FlushAsync();
        }
        else if (request[0] == '2')
        {
            var streamWriter = new StreamWriter(networkStream, Encoding.Latin1);
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
        var count = 0;
        var stringBuilder = new StringBuilder();

        foreach (var subdirectory in Directory.GetDirectories(path))
        {
            stringBuilder.Append($" ({Path.GetFileName(subdirectory)} true)");
            ++count;
        }

        foreach (var file in Directory.GetFiles(path))
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

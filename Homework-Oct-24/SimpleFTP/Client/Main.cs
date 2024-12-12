// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1200 // Using directives should be placed correctly
using System.Net.Sockets;
using static Client.Client;
#pragma warning restore SA1200 // Using directives should be placed correctly

const int Port = 1234;

const int ListRequestType = 1;
const int GetRequestType = 2;

var (requestType, path) = ValidateArgs();
using var tcpClient = new TcpClient("localhost", Port);
var networkStream = tcpClient.GetStream();
Console.WriteLine("The connection has been established");

switch (requestType)
{
    case ListRequestType:
        var response = await ProcessListRequest(networkStream, path);
        Console.WriteLine($"The response: {response}");
        break;
    case GetRequestType:
        var bytes = await ProcessGetRequest(networkStream, path);
        var fileName = Path.GetFileName(path);
        await File.WriteAllBytesAsync(fileName, bytes);
        Console.WriteLine($"The file was received and written to the path: {fileName} in the current working directory");
        break;
    default:
        throw new InvalidProgramException();
}

(int requestType, string path) ValidateArgs()
{
    if (args.Length != 2)
    {
        throw new InvalidDataException("The request must contain two arguments");
    }

    int requestType = args[0] switch
    {
        "List" => ListRequestType,
        "Get" => GetRequestType,
        _ => throw new InvalidDataException("The request command must be either 'List' or 'Get'")
    };
    string path = args[1];
    return (requestType, path);
}

// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1200 // Using directives should be placed correctly
using System.Net;
using System.Net.Sockets;
using static Server.Server;
#pragma warning restore SA1200 // Using directives should be placed correctly

const int Port = 1234;

using var tcpListener = new TcpListener(IPAddress.Any, Port);
tcpListener.Start();
Console.WriteLine("The server has been started");

while (true)
{
    var tcpClient = await tcpListener.AcceptTcpClientAsync();
    var task = // To avoid compiler warning
        Task.Run(async () =>
        {
            using var networkStream = tcpClient.GetStream();
            await ProcessClient(networkStream);
        });
}

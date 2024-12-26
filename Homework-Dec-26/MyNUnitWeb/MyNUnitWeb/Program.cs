// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1200 // Using directives should be placed correctly
using MyNUnitWeb;
#pragma warning restore SA1200 // Using directives should be placed correctly

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(hubOptions =>
    {
        hubOptions.MaximumReceiveMessageSize = 2_000_000;
    });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<MyNUnitHub>("/MyNUnit");

app.Run();

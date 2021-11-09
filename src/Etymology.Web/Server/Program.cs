using Etymology.Web.Server;
using Microsoft.AspNetCore;

using IWebHost host = WebHost.CreateDefaultBuilder<Startup>(args).Build();
await host.RunAsync();

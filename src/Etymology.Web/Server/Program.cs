using Etymology.Web.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using IWebHost host = WebHost.CreateDefaultBuilder<Startup>(args).Build();
await host.RunAsync();

using Etymology.Web.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

WebHost
    .CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .Build()
    .Run();

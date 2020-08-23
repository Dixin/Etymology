namespace Etymology.Web.Server
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    internal static class Program
    {
        private static void Main(string[] args) => BuildWebHost(args).Run();

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}

using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Exceptions;
using UploadService.Configurations;

namespace UploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 40000000;
                    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                })
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithDemystifiedStackTraces()
                    .Enrich.WithExceptionDetails()
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IO;

namespace ocelothost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
              .UseKestrel()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config
                      .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                      .AddJsonFile("appsettings.json", true, true)
                      .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                      .AddJsonFile("ocelot.json")
                      .AddEnvironmentVariables();
              })
              .ConfigureServices(services => {
                  var identityUrl = "http://localhost:5000";
                  var authenticationProviderKey = "IdentityApiKey";
                  string scope = "api1";

                  services.AddAuthentication("Bearer")
                      .AddJwtBearer(authenticationProviderKey, x =>
                      {
                          x.Authority = identityUrl;
                          x.RequireHttpsMetadata = false;
                          x.Audience = scope;
                    //x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    //{
                    //    ValidAudiences = new[] { "api1" }
                    //};
                });
                  services.AddOcelot();
              })
              .ConfigureLogging((hostingContext, loggingbuilder) =>
              {
                  loggingbuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  loggingbuilder.AddConsole();
                  loggingbuilder.AddDebug();
              })
              .UseIISIntegration()//For In-Process hosting, replace to .UseIIS()
              .Configure(app =>
              {
                  app.UseOcelot().Wait();
              })
              .Build()
              .Run();
            //CreateWebHostBuilder(args).Build().Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //         .ConfigureAppConfiguration((hostingContext, config) =>
        //         {
        //             config
        //                 .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        //                 .AddJsonFile("appsettings.json", true, true)
        //                 .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
        //                 .AddJsonFile("ocelot.json") // or use ".AddOcelot(hostingContext.HostingEnvironment)" to look for pattern json file https://ocelot.readthedocs.io/en/latest/features/configuration.html
        //                 .AddEnvironmentVariables();
        //         })
        //        .UseStartup<Startup>();
    }
}

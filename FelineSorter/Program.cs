using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using FelineSorter.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FelineSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            Console.Write("Press enter to exit...");
            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddInMemoryCollection();
            var configuration = builder.Build();

            var webserviceOptions = new WebserviceOptions();
            configuration.Bind("WebserviceOptions", webserviceOptions);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<Consumer>().As<IConsumer>().SingleInstance();
            containerBuilder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            containerBuilder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));
            containerBuilder.RegisterType<HttpClientWrapper>().As<IHttpClient>();

            // potentially register more sorters here, can rename the existing sorter to something a bit more specific later if required
            containerBuilder.RegisterType<FelineOwnerSorter>().As<IFelineOwnerSorter>();
            containerBuilder.RegisterInstance(webserviceOptions);

            var container = containerBuilder.Build();
            container.Resolve<ILoggerFactory>().AddFile("FelineSorter-{Date}.log");

            // uncomment this to log to console
            //container.Resolve<ILoggerFactory>().AddConsole(Enum.Parse<LogLevel>(configuration["LogLevel"], true));

            var consumer = container.Resolve<IConsumer>();
            await consumer.Consume();
        }
    }
}

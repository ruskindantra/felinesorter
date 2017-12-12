using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FelineSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddInMemoryCollection();
            var configuration = builder.Build();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<Consumer>().As<IConsumer>().SingleInstance();
            containerBuilder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            containerBuilder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            var container = containerBuilder.Build();
            container.Resolve<ILoggerFactory>().AddConsole(Enum.Parse<LogLevel>(configuration["LogLevel"], true));

            var consumer = container.Resolve<IConsumer>();
            consumer.Consume();

            Console.Write("Press enter to exit...");
            Console.ReadLine();
        }
    }
}

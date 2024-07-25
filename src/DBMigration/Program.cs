using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Employee.Service.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a connection string as a command-line argument.");
                return;
            }

            var serviceProvider = CreateServices(args[0]);

            using var scope = serviceProvider.CreateScope();
            UpdateDatabase(scope.ServiceProvider);
        }

        private static IServiceProvider CreateServices(string dbConnectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2012()
                    .WithGlobalConnectionString(
                        dbConnectionString)
                    .ScanIn(typeof(Program).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluginDemo.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing plugins...");

            ExecutePlugins();
            ExecuteDIPlugin();
        }

        private static void ExecutePlugins()
        {
            string[] pluginPaths = new string[]
                {
                    @"JsonPlugin\bin\Debug\netcoreapp3.0\JsonPlugin.dll",
                    @"JsonPluginOld\bin\Debug\netstandard2.0\JsonPluginOld.dll",
                    //@"JsonPlugin\bin\Release\netstandard2.0\JsonPlugin.dll"
                };

            IEnumerable<ICommand> commands = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreateCommands(pluginAssembly);
            }).ToList();

            Console.WriteLine("Commands: ");
            foreach (ICommand command in commands)
            {
                Console.WriteLine($"{command.Name}\t - {command.Description}");
                command.Execute();
            }
        }

        private static void ExecuteDIPlugin()
        {
            string[] pluginPaths = new string[]
                {
                    @"DIPlugin\bin\Debug\netstandard2.0\DIPlugin.dll"
                };

            ICommandFactory commandFactory = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreateCommandFactories(pluginAssembly);
            }).FirstOrDefault();

            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDependantService, DependantService>();
            
            services.AddSingleton<ICommandFactory>(serviceProvider =>
            {
                var dependantService = serviceProvider.GetRequiredService<IDependantService>();
                commandFactory.ConfigureServices(dependantService);
                return commandFactory;
            });

            services.AddTransient<ICommand>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<ICommandFactory>();
                return factory.Create();
            });
            var serviceProvider = services.BuildServiceProvider();

            var command = serviceProvider.GetRequiredService<ICommand>();
            command.Execute();
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<ICommand> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type))
                {
                    ICommand result = Activator.CreateInstance(type) as ICommand;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        static IEnumerable<ICommandFactory> CreateCommandFactories(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommandFactory).IsAssignableFrom(type))
                {
                    ICommandFactory result = Activator.CreateInstance(type) as ICommandFactory;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}

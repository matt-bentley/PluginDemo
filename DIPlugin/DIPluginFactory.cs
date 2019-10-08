using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluginDemo.Core;
using System;
using System.IO;

namespace DIPlugin
{
    public class DIPluginFactory : ICommandFactory
    {
        private IServiceProvider _serviceProvider;

        public DIPluginFactory()
        {
            
        }

        public string Name => "di";
        public string Description => "Creates plugin using DI.";

        public void ConfigureServices(IDependantService dependantService)
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.DIPlugin.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDependantService>(dependantService);
            services.AddSingleton<ICommand, DIPlugin>();
            _serviceProvider = services.BuildServiceProvider();
        }

        public ICommand Create()
        {
            return _serviceProvider.GetRequiredService<ICommand>();
        }
    }
}

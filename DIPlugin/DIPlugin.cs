using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PluginDemo.Core;
using System;
using System.Reflection;

namespace DIPlugin
{
    public class DIPlugin : ICommand
    {
        private readonly IConfiguration Configuration;
        private readonly IDependantService _dependantService;

        public DIPlugin(IConfiguration configuration, IDependantService dependantService)
        {
            this.Configuration = configuration;
            _dependantService = dependantService;
        }

        public string Name => "di";
        public string Description => "Creates plugin using DI.";

        private struct Info
        {
            public string JsonVersion;
            public string JsonLocation;
            public string Machine;
            public string User;
            public DateTime Date;
        }

        public int Execute()
        {
            Assembly jsonAssembly = typeof(JsonConvert).Assembly;
            Info info = new Info()
            {
                JsonVersion = jsonAssembly.FullName,
                JsonLocation = jsonAssembly.Location,
                Machine = Environment.MachineName,
                User = Environment.UserName,
                Date = DateTime.Now
            };

            Console.WriteLine(JsonConvert.SerializeObject(info, Formatting.Indented));

            Console.Write($"TestSetting: {Configuration["TestSetting"]}");
            Console.Write($"Username: {_dependantService.GetUsername()}");

            return 0;
        }
    }
}

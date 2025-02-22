﻿using Newtonsoft.Json;
using PluginDemo.Core;
using System;
using System.Reflection;

namespace JsonPluginOld
{
    public class JsonPluginOld : ICommand
    {
        public string Name => "old json";

        public string Description => "Outputs JSON value (Old).";

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

            return 0;
        }
    }
}

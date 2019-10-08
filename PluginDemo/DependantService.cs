using Microsoft.Extensions.Configuration;
using PluginDemo.Core;

namespace PluginDemo
{
    public class DependantService : IDependantService
    {
        private readonly IConfiguration Configuration;

        public DependantService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public string GetUsername()
        {
            return Configuration["OverrideUser"];
        }
    }
}

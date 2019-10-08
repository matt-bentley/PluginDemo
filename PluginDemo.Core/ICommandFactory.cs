
namespace PluginDemo.Core
{
    public interface ICommandFactory
    {
        string Name { get; }
        string Description { get; }

        void ConfigureServices(IDependantService dependantService);
        ICommand Create();
    }
}

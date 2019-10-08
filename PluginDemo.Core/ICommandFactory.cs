
namespace PluginDemo.Core
{
    public interface ICommandFactory
    {
        string Name { get; }
        string Description { get; }

        ICommand Create();
    }
}

using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Commands
{
    public interface ICommand
    {
        Result Execute();
    }
}
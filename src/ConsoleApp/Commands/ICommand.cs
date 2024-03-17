using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Commands
{
    public interface ICommand<T>
    {
        Result<T> Execute();
    }
}
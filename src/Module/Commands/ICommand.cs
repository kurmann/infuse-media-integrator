using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Module.Commands;

public interface ICommand<T>
{
    Result<T> Execute();
}
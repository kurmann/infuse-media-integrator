using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Module.Queries;

public interface IQueryService<T>
{
    public Result<T> Execute();
}
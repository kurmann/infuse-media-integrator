using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public interface IQueryService<T>
{
    public Result<T> Execute();
}
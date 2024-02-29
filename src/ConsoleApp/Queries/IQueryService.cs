using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public interface IQueryService<T>
{
    public IReadOnlyList<Result<IReadOnlyList<T>>> Execute();
}
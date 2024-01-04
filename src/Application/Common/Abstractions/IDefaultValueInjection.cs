using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.Abstractions;

public interface IDefaultValueInjection
{
    T Inject<T>(T model) where T : EntityBase;
    List<T> Inject<T>(List<T> entities) where T : EntityBase;
    T InjectFromExisting<T>(T model, T source) where T : EntityBase;
}
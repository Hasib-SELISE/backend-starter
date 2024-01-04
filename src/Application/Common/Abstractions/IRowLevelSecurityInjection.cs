using Selise.Ecap.Entities.PrimaryEntities.PlatformSpecific;
using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.Abstractions;

public interface IRowLevelSecurityInjection
{
    T Inject<T>(T entity) where T : EntityBase;
    List<T> Inject<T>(List<T> entities) where T : EntityBase;
    T InjectIdsFromAnotherEntity<T, TSource>(T entity, TSource sourceEntity) where T : EntityBase
        where TSource : EntityBase;
    void InjectPersonIdsAllowedToRead<T>(T entity
        , IEnumerable<string> personids, IEnumerable<string> injectableFields) where T : EntityBase;
    void InjectIdsAllowedToRead<T>(T entity
        , IEnumerable<string> userids, IEnumerable<string> injectableFields) where T : EntityBase;
    EntityDefaultPermissionSettings GetSettings(string entityName);
}
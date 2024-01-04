using Application.Common.Abstractions;
using MongoDB.Driver;
using Selise.Ecap.Entities.PrimaryEntities.PlatformSpecific;
using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.DefaultValueInjection;

public class RowLevelSecurityInjection : IRowLevelSecurityInjection
{
    private readonly IRmwRepository _repository;
    private readonly RoleInterpreter _roleInterpreter;

    public RowLevelSecurityInjection(IRmwRepository repository, RoleInterpreter roleInterpreter)
    {
        _repository = repository;
        _roleInterpreter = roleInterpreter;
    }

    public T Inject<T>(T entity) where T : EntityBase
    {
        if (entity == null) throw new Exception("No entity found to add row level security");
        var entityName = entity.GetType().Name;

        var settings = GetSettings(entityName);
        if (settings == null)
            throw new Exception(
                $"Row level security setting is missing for entity = {entityName}. Please check your tenant's EntityDefaultPermissionSettingss document");

        entity.RolesAllowedToRead = GetInterpretedRoles(settings.RolesAllowedToRead, entity.RolesAllowedToRead);
        entity.IdsAllowedToRead = GetInterpretedIds(settings.IdsAllowedToRead, entity.IdsAllowedToRead);

        entity.RolesAllowedToUpdate = GetInterpretedRoles(settings.RolesAllowedToUpdate, entity.RolesAllowedToUpdate);
        entity.IdsAllowedToUpdate = GetInterpretedIds(settings.IdsAllowedToUpdate, entity.IdsAllowedToUpdate);

        entity.RolesAllowedToDelete = GetInterpretedRoles(settings.RolesAllowedToDelete, entity.RolesAllowedToDelete);
        entity.IdsAllowedToDelete = GetInterpretedIds(settings.IdsAllowedToDelete, entity.IdsAllowedToDelete);

        return entity;
    }

    public List<T> Inject<T>(List<T> entities) where T : EntityBase
    {
        var rowlevelSecurityForFirstItem = entities.First();
        rowlevelSecurityForFirstItem = this.Inject(rowlevelSecurityForFirstItem);

        foreach (var entity in entities)
        {
            entity.RolesAllowedToRead = rowlevelSecurityForFirstItem.RolesAllowedToRead;
            entity.IdsAllowedToRead = rowlevelSecurityForFirstItem.IdsAllowedToRead;
            entity.RolesAllowedToUpdate = rowlevelSecurityForFirstItem.RolesAllowedToUpdate;
            entity.IdsAllowedToUpdate = rowlevelSecurityForFirstItem.IdsAllowedToUpdate;
            entity.RolesAllowedToDelete = rowlevelSecurityForFirstItem.RolesAllowedToDelete;
            entity.IdsAllowedToDelete = rowlevelSecurityForFirstItem.IdsAllowedToDelete;
        }

        return entities;
    }

    public T InjectIdsFromAnotherEntity<T, TSource>(T entity, TSource sourceEntity) where T : EntityBase
        where TSource : EntityBase
    {
        entity.IdsAllowedToDelete = sourceEntity.IdsAllowedToDelete;
        entity.IdsAllowedToRead = sourceEntity.IdsAllowedToRead;
        entity.IdsAllowedToUpdate = sourceEntity.IdsAllowedToUpdate;
        entity.IdsAllowedToWrite = sourceEntity.IdsAllowedToWrite;
        return entity;
    }

    public void InjectPersonIdsAllowedToRead<T>(T entity
        , IEnumerable<string> personIds, IEnumerable<string> injectableFields) where T : EntityBase
    {
        try
        {
            var connections = _repository
                .GetItems<Connection>(x => personIds.Contains(x.ChildEntityID) && x.ChildEntityName == "Person")
                .ToList();
            var userIds = connections.Where(x => x.ParentEntityName == "User").Select(x => x.ParentEntityID).ToArray();

            if (!userIds.Any())
            {
                return;
            }

            InjectIdsAllowedToRead(entity, userIds, injectableFields);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public void InjectIdsAllowedToRead<T>(
        T entity,
        IEnumerable<string> userIds,
        IEnumerable<string> injectableFields) where T : EntityBase
    {
        if (userIds == null)
        {
            return;
        }

        try
        {
            if (injectableFields.Contains("IdsAllowedToRead"))
            {
                entity.IdsAllowedToRead = PrepareIdArrayToAllow(entity.IdsAllowedToRead, userIds);
            }

            if (injectableFields.Contains("IdsAllowedToUpdate"))
            {
                entity.IdsAllowedToUpdate = PrepareIdArrayToAllow(entity.IdsAllowedToUpdate, userIds);
            }

            if (injectableFields.Contains("IdsAllowedToRead"))
            {
                entity.IdsAllowedToDelete = PrepareIdArrayToAllow(entity.IdsAllowedToDelete, userIds);
            }
        }
        catch (Exception)
        {
        }
    }


    private string[] PrepareIdArrayToAllow(IEnumerable<string> fieldValues, IEnumerable<string> ids)
    {
        var idsAllowed = fieldValues.ToList();
        foreach (var item in ids)
        {
            if (!idsAllowed.Contains(item))
            {
                idsAllowed.Add(item);
            }
        }

        return idsAllowed.ToArray();
    }

    public EntityDefaultPermissionSettings GetSettings(string entityName)
    {
        return _repository.GetItem<EntityDefaultPermissionSettings>(p => p.EntityName == entityName);
    }

    private string[] GetInterpretedRoles(string[] rolesInSettings, string[] existingRoles)
    {
        if (rolesInSettings == null) return new string[] { };
        var data = rolesInSettings.SelectMany(rolesAllowedToRead => _roleInterpreter.InterpretRoles(rolesAllowedToRead))
            .ToList();

        if (existingRoles != null && existingRoles.Any())
            data.AddRange(existingRoles);

        return data.Distinct().ToArray();
    }

    private string[] GetInterpretedIds(string[] idsInSettings, string[] existingIds)
    {
        if (idsInSettings == null) return new string[] { };
        var data = idsInSettings
            .SelectMany(rolesAllowedToRead => _roleInterpreter.InterpretIds(rolesAllowedToRead))
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        if (existingIds != null && existingIds.Any())
            data.AddRange(existingIds);

        return data.Distinct().ToArray();
    }
}

public class Roles
{
    public static readonly string[] SystemDefinedRoles = new string[]
    {
        "anonymous",
        "admin",
        "appuser",
        "nwp_user",
        "customer_user",
        "nwp_manager"
    };
}
using Application.Common.Abstractions;
using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.DefaultValueInjection;

public class DefaultValueInjection : IDefaultValueInjection
{
    private readonly IRmwSecurityContextProvider _securityContextProvider;

    public DefaultValueInjection(IRmwSecurityContextProvider securityDataProvider)
    {
        _securityContextProvider = securityDataProvider;
    }

    public T Inject<T>(T model) where T : EntityBase
    {
        var authData = _securityContextProvider.GetSecurityContext();

        model.TenantId = authData.TenantId;
        model.LastUpdatedBy = authData.UserId;
        model.LastUpdateDate = DateTime.UtcNow;
        if (string.IsNullOrEmpty(model.Language))
            model.Language = authData.Language;

        if (string.IsNullOrEmpty(model.CreatedBy))
        {
            model.CreatedBy = authData.UserId;
        }

        if (string.IsNullOrWhiteSpace(model.ItemId))
        {
            model.ItemId = Guid.NewGuid().ToString();
        }

        if (model.Tags == null || !model.Tags.Any())
        {
            model.Tags = new[] { model.GetType().Name };
        }

        model.CreatedBy = authData.UserId;

        if (model.CreateDate == DateTime.MinValue)
            model.CreateDate = DateTime.UtcNow;

        return model;
    }

    public List<T> Inject<T>(List<T> entities) where T : EntityBase
    {
        if (entities == null || !entities.Any()) return entities;

        foreach (var entity in entities.Where(entity => entity != null))
        {
            Inject(entity);
        }

        return entities;
    }

    public T InjectFromExisting<T>(T model, T source) where T : EntityBase
    {
        var authData = _securityContextProvider.GetSecurityContext();

        model.LastUpdatedBy = authData.UserId;
        model.LastUpdateDate = DateTime.UtcNow;

        model.TenantId = source.TenantId;
        model.CreatedBy = source.CreatedBy;
        model.CreateDate = source.CreateDate;
        model.ItemId = source.ItemId;
        model.Language = authData.Language;
        model.Tags = new[] { model.GetType().Name };

        return model;
    }
}
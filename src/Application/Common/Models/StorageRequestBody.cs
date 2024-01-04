using Selise.Ecap.Entities.PrimaryEntities.StorageService;

namespace Application.Common.Models;

public class StorageRequestBody
{
    public string ItemId { get; set; }
    public string MetaData
    {
        get;
        set;
    }
    public string Name { get; set; }
    public string ParentDirectoryId { get; set; }
    public string Tags { get; set; }
    public AccessModifier? AccessModifier { get; set; }

    public StorageRequestBody()
    {
        //AccessModifier = new AccessModifier();
    }
}
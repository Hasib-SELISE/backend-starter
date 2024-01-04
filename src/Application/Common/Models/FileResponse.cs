namespace Application.Common.Models;

public class FileResponse
{
    public string Url
    {
        get;
        set;
    }

    public int AccessModifier
    {
        get;
        set;
    }

    public string ItemId
    {
        get;
        set;
    }

    public string[] Tags
    {
        get;
        set;
    }

    public Dictionary<string, FileMetaDataResponse> MetaData
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public string ParentDirectoryID
    {
        get;
        set;
    }

    public string SystemName
    {
        get;
        set;
    }

    public int Type
    {
        get;
        set;
    }

    public string TypeString
    {
        get;
        set;
    }

    public DateTime CreateDate
    {
        get;
        set;
    }

    public string CreatedBy
    {
        get;
        set;
    }

    public string Language
    {
        get;
        set;
    }

    public string TenantId
    {
        get;
        set;
    }

    public long SizeInBytes
    {
        get;
        set;
    }

    public bool Exists => true;
}
namespace Application.Common.Models.ResponseModels;

public class GenericPaginationResponseModel<T> where T : class
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    //public int TotalPages => TotalData == 0 ? 0 : (int)Math.Ceiling((double)(TotalData / PageSize));
    public long TotalData { get; set; }
    public List<T> Data { get; set; }
    public GenericPaginationResponseModel()
    {
        Data = new List<T>();
    }
    public GenericPaginationResponseModel(int pageNumber, int pageSize, long totaldata, List<T> data)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalData = totaldata;
    }
    public GenericPaginationResponseModel(int pageNumber, int pageSize, List<T> data)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    public GenericPaginationResponseModel<T> SetDataFromLodedData()
    {
        TotalData = Data.Count;
        Data = Data.Skip(PageNumber * PageSize).Take(PageSize).ToList();
        return this;
    }
    public GenericPaginationResponseModel<T> SetDataFromLodedData(int pageNumber, int pageSize, long totaldata, List<T> data)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalData = totaldata;
        return this;
    }
}
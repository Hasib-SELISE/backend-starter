namespace Application.Common.Models;

public class BasePaginationQueryModel
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public BasePaginationQueryModel()
    {
        if (PageSize <= 0)
        {
            PageSize = 10;
        }
    }
}
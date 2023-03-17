namespace WepApiTest.Core;

public class RequestParams
{
    const int maxPageSize = 20;

    public int PageNumber { get; set; } = 1;

    public int _pageSize = 10;
    public int PageSize { get => _pageSize; set => _pageSize = (value > maxPageSize) ? maxPageSize : value; }
}

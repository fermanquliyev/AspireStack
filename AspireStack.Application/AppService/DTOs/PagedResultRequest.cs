namespace AspireStack.Application.AppService.DTOs
{
    public class PagedResultRequest: IPagedResultRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

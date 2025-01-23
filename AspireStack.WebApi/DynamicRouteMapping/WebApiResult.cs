namespace AspireStack.WebApi.DynamicRouteMapping
{
    public class WebApiResult
    {
        public object? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}

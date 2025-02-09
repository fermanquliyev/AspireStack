namespace AspireStack.WebApi.DynamicRouteMapping
{
    public class WebApiResult: WebApiResult<object>
    {
    }

    public class WebApiResult<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}

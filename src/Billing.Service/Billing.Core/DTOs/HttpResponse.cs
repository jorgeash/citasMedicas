namespace Billing.Core.DTOs;

public class HttpResponse<T>
{
    public T Data { get; set; }

    public HttpResponse(T data)
    {
        Data = data;
    }
}

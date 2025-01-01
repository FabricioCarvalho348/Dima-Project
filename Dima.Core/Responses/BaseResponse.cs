using System.Text.Json.Serialization;

namespace Dima.Core.Responses;

public class BaseResponse<TData>
{
    private readonly int _code;

    [JsonConstructor]
    public BaseResponse() => _code = Configuration.DefaultStatusCode;
    public BaseResponse(
        TData? data, 
        int code = Configuration.DefaultStatusCode, 
        string? message = null)
    {
        Data = data;
        _code = code;
        Message = message;
    }

    public TData? Data { get; set; }
    public string? Message { get; set; }
    
    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}
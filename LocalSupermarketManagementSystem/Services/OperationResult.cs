namespace LocalSupermarketManagementSystem.Services;

public class OperationResult
{
    public bool Success { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    public static OperationResult Ok(string message) => new() { Success = true, Message = message };
    public static OperationResult Fail(string message) => new() { Success = false, Message = message };
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }

    public static OperationResult<T> Ok(T data, string message) => new() { Success = true, Message = message, Data = data };
    public new static OperationResult<T> Fail(string message) => new() { Success = false, Message = message };
}

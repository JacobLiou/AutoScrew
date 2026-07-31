namespace AutoScrew.Infrastructure.Mes.ProductKey.Models;

public sealed class MesResult<T>
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public T? Data { get; init; }

    public static MesResult<T> Ok(T data) => new() { Success = true, Data = data };

    public static MesResult<T> Fail(string error) => new() { Success = false, Error = error };
}

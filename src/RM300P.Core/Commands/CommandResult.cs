using RM300P.Core.Protocol;

namespace RM300P.Core.Commands;

public sealed class CommandResult<T>
{
    public StatusCode Status    { get; init; }
    public bool       IsSuccess => Status == StatusCode.Success;
    public T?         Data      { get; init; }
    public Packet     Raw       { get; init; } = null!;

    public static CommandResult<T> Ok(T data, Packet raw) =>
        new() { Status = StatusCode.Success, Data = data, Raw = raw };

    public static CommandResult<T> Fail(StatusCode status, Packet raw) =>
        new() { Status = status, Raw = raw };
}

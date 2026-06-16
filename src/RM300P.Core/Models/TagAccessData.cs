using RM300P.Core.Protocol;

namespace RM300P.Core.Models;

public sealed class TagAccessData
{
    public byte    OpId      { get; init; }
    public byte    ErrorCode { get; init; }
    public byte[]  Data      { get; init; } = Array.Empty<byte>();
    public bool    IsSuccess => ErrorCode == 0x00;

    public string ErrorDescription => ErrorCode switch
    {
        0x00 => "成功",
        0x01 => "無標籤",
        0x02 => "密碼錯誤",
        0x03 => "已鎖定",
        0x04 => "記憶體過長",
        _    => $"錯誤碼 0x{ErrorCode:X2}"
    };

    public static TagAccessData Parse(Packet p)
    {
        if (p.Payload.Length < 3)
            return new TagAccessData();

        return new TagAccessData
        {
            OpId      = p.Payload[1],
            ErrorCode = p.Payload[2],
            Data      = p.Payload.Length > 3 ? p.Payload[3..] : Array.Empty<byte>()
        };
    }
}

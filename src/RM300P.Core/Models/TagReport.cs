using RM300P.Core.Protocol;

namespace RM300P.Core.Models;

public sealed class TagReport
{
    public ushort Id      { get; init; }
    public byte   AntId  { get; init; }
    public short  Rssi   { get; init; }   // cdBm
    public byte[] Epc    { get; init; } = Array.Empty<byte>();
    public byte[]? Tid   { get; init; }
    public byte[] Raw    { get; init; } = Array.Empty<byte>();

    public string EpcHex => BitConverter.ToString(Epc).Replace("-", " ");

    public static TagReport Parse(Packet p)
    {
        // 基本解析（0xC0）：STATUS(1) + ANTID(1) + RSSI(2,signed,BE) + PC(2) + EPC(n)
        // TID/相位格式（0xC1/0xC2/0xB0~0xB2）留 raw 供進階解析
        if (p.Payload.Length < 5)
            return new TagReport { Id = p.Id, Raw = p.Raw };

        byte antId = p.Payload[1];
        short rssi = (short)((p.Payload[2] << 8) | p.Payload[3]);
        // PC(2) at [4..5]，EPC 從 [6] 開始
        byte[] epc = p.Payload.Length > 6 ? p.Payload[6..] : Array.Empty<byte>();

        return new TagReport
        {
            Id    = p.Id,
            AntId = antId,
            Rssi  = rssi,
            Epc   = epc,
            Raw   = p.Raw
        };
    }
}

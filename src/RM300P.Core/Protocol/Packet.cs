namespace RM300P.Core.Protocol;

/// <summary>
/// 一筆完整封包。Raw 含 STX..ETX，Payload 含 STATUS+DATA（回應）或 DATA（送出）。
/// </summary>
public sealed class Packet
{
    public ushort Id      { get; }
    public byte[] Payload { get; }
    public byte[] Raw     { get; }

    public Packet(ushort id, byte[] payload, byte[] raw)
    {
        Id      = id;
        Payload = payload;
        Raw     = raw;
    }
}

namespace RM300P.Core.Protocol;

/// <summary>
/// 主機指令封包編碼器。
/// 格式（無 DATA）：STX | LENGTH(2,BE) | ID(2,BE) | CHECKSUM(2,BE) | ETX
/// 格式（有 DATA）：STX | LENGTH(2,BE) | ID(2,BE) | DATA | CHECKSUM(2,BE) | ETX
/// LENGTH = 2(LENGTH本身) + 2(ID) + len(DATA)
/// </summary>
public static class PacketCodec
{
    private const byte Stx = 0x02;
    private const byte Etx = 0x03;

    public static byte[] Encode(ushort id, ReadOnlySpan<byte> data = default)
    {
        int dataLen    = data.Length;
        int lengthVal  = 2 + 2 + dataLen;          // LENGTH(2) + ID(2) + DATA
        int totalBytes = 1 + 2 + 2 + dataLen + 2 + 1; // STX + LENGTH + ID + DATA + CHECKSUM + ETX

        byte[] buf = new byte[totalBytes];
        int pos = 0;

        buf[pos++] = Stx;
        buf[pos++] = (byte)(lengthVal >> 8);
        buf[pos++] = (byte)(lengthVal & 0xFF);
        buf[pos++] = (byte)(id >> 8);
        buf[pos++] = (byte)(id & 0xFF);

        foreach (byte b in data)
            buf[pos++] = b;

        ushort cs = Checksum.Calc(buf.AsSpan(1, lengthVal));
        buf[pos++] = (byte)(cs >> 8);
        buf[pos++] = (byte)(cs & 0xFF);
        buf[pos]   = Etx;

        return buf;
    }
}

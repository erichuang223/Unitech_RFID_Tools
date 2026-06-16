namespace RM300P.Core.Protocol;

public static class Checksum
{
    /// <summary>
    /// 輸入區 = LENGTH(2) + ID(2) + STATUS(若有) + DATA，即封包去掉 STX/CHECKSUM/ETX 後的部分。
    /// CHECKSUM = (0x10000 - (Σ bytes &amp; 0xFFFF)) &amp; 0xFFFF（16-bit 二補數，大端序輸出）
    /// </summary>
    public static ushort Calc(ReadOnlySpan<byte> lengthToDataInclusive)
    {
        int sum = 0;
        foreach (byte b in lengthToDataInclusive) sum += b;
        return (ushort)((0x10000 - (sum & 0xFFFF)) & 0xFFFF);
    }
}

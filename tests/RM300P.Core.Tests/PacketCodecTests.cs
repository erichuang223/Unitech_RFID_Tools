using RM300P.Core.Protocol;

namespace RM300P.Core.Tests;

/// <summary>
/// 測試規格 04 階段 -1.2：封包編碼正確
/// </summary>
public class PacketCodecTests
{
    [Fact]
    public void Encode_ReadFirmwareVersion_MatchesExpected()
    {
        // [2.1] 讀版本 0x21 無 DATA：期望 02 00 04 00 21 FF DB 03
        byte[] expected = [0x02, 0x00, 0x04, 0x00, 0x21, 0xFF, 0xDB, 0x03];
        byte[] actual   = PacketCodec.Encode(0x0021);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Encode_StartsWithStx_EndsWithEtx()
    {
        byte[] packet = PacketCodec.Encode(0x0080);
        Assert.Equal(0x02, packet[0]);
        Assert.Equal(0x03, packet[^1]);
    }

    [Fact]
    public void Encode_WithData_LengthFieldCorrect()
    {
        // LENGTH = 2(LENGTH) + 2(ID) + len(DATA)
        byte[] data   = [0x01, 0x02, 0x03];
        byte[] packet = PacketCodec.Encode(0x0117, data);
        int    lengthVal = (packet[1] << 8) | packet[2];
        Assert.Equal(2 + 2 + data.Length, lengthVal);
    }

    [Fact]
    public void Encode_WithData_ChecksumValid()
    {
        byte[] data   = [0xAA, 0xBB];
        byte[] packet = PacketCodec.Encode(0x0101, data);
        int    lengthVal = (packet[1] << 8) | packet[2];
        ushort expected  = Checksum.Calc(packet.AsSpan(1, lengthVal));
        ushort actual    = (ushort)((packet[^3] << 8) | packet[^2]);
        Assert.Equal(expected, actual);
    }
}

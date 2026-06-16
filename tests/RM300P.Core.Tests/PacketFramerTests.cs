using RM300P.Core.Protocol;
using RM300P.Core.Transport;

namespace RM300P.Core.Tests;

/// <summary>
/// 測試規格 04 階段 -1.3：分包器正確——含 DATA 夾帶 0x02/0x03、連黏、分段、雜訊前綴
/// </summary>
public class PacketFramerTests
{
    // 讀版本 0x21 指令封包（已驗算）
    private static readonly byte[] VersionCmdPacket =
        [0x02, 0x00, 0x04, 0x00, 0x21, 0xFF, 0xDB, 0x03];

    // Inventory Data 封包（DATA 中含 0x03 在 index 14）
    // STX(02) LENGTH(00 15) ID(00 C0) STATUS(01) DATA(E9 62 34 00 AA AA 34 12 DC [03] 01 17 16 10 B9 88) CS(F9 B3) ETX(03)
    private static readonly byte[] InventoryPacket =
    [
        0x02, 0x00, 0x15, 0x00, 0xC0, 0x01, 0xE9, 0x62,
        0x34, 0x00, 0xAA, 0xAA, 0x34, 0x12, 0xDC, 0x03,
        0x01, 0x17, 0x16, 0x10, 0xB9, 0x88, 0xF9, 0xB3, 0x03
    ];

    private static PacketFramer MakeFramer(out List<Packet> received)
    {
        var list = new List<Packet>();
        received = list;
        return new PacketFramer(pkt => list.Add(pkt));
    }

    [Fact]
    public void Feed_SinglePacket_ExtractsOne()
    {
        var framer = MakeFramer(out var received);
        framer.Feed(VersionCmdPacket);
        Assert.Single(received);
        Assert.Equal(0x0021, received[0].Id);
    }

    [Fact]
    public void Feed_TwoConcatenatedPackets_ExtractsBoth()
    {
        var framer = MakeFramer(out var received);
        byte[] concat = [..VersionCmdPacket, ..VersionCmdPacket];
        framer.Feed(concat);
        Assert.Equal(2, received.Count);
    }

    [Fact]
    public void Feed_FragmentedPacket_WaitsAndExtractsWhenComplete()
    {
        var framer = MakeFramer(out var received);
        framer.Feed(VersionCmdPacket[..4]);   // 餵一半
        Assert.Empty(received);
        framer.Feed(VersionCmdPacket[4..]);   // 餵後半
        Assert.Single(received);
    }

    [Fact]
    public void Feed_NoisePrefixBeforeStx_DiscardedAndExtractsPacket()
    {
        var framer = MakeFramer(out var received);
        byte[] noisy = [0xAA, 0xBB, 0xCC, ..VersionCmdPacket];
        framer.Feed(noisy);
        Assert.Single(received);
        Assert.Equal(0x0021, received[0].Id);
    }

    [Fact]
    public void Feed_DataContaining0x03_NotConfusedAsEtx()
    {
        // Inventory 封包 DATA 中有 0x03（index 15），分包器不應在此截斷
        var framer = MakeFramer(out var received);
        framer.Feed(InventoryPacket);
        Assert.Single(received);
        Assert.Equal(0x00C0, received[0].Id);
    }

    [Fact]
    public void Feed_DataContaining0x02_NotConfusedAsStx()
    {
        // 建立含 0x02 在 DATA 中的合法封包
        byte[] data   = [0x00, 0x02, 0xFF];   // DATA 中含 0x02
        byte[] packet = PacketCodec.Encode(0x0100, data);
        var framer = MakeFramer(out var received);
        framer.Feed(packet);
        Assert.Single(received);
        Assert.Equal(0x0100, received[0].Id);
    }

    [Fact]
    public void Feed_InvalidChecksum_Discarded()
    {
        var errors  = new List<string>();
        var packets = new List<Packet>();
        var framer  = new PacketFramer(p => packets.Add(p), e => errors.Add(e));

        byte[] bad = VersionCmdPacket.ToArray();
        bad[5] ^= 0xFF;   // 破壞 CHECKSUM
        framer.Feed(bad);

        Assert.Empty(packets);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Feed_ValidPacket_RawBytesPreserved()
    {
        var framer = MakeFramer(out var received);
        framer.Feed(VersionCmdPacket);
        Assert.Equal(VersionCmdPacket, received[0].Raw);
    }
}

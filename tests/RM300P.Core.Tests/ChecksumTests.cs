using RM300P.Core.Protocol;

namespace RM300P.Core.Tests;

/// <summary>
/// 測試規格 04 階段 -1.1：CHECKSUM 算法正確（三組原文範例向量）
/// </summary>
public class ChecksumTests
{
    [Fact]
    public void Calc_ReadFirmwareVersion_Command()
    {
        // [2.1] Host 讀版本指令：LENGTH(00 04) + ID(00 21) → sum=0x25 → CS=0xFFDB
        byte[] input = [0x00, 0x04, 0x00, 0x21];
        Assert.Equal(0xFFDB, Checksum.Calc(input));
    }

    [Fact]
    public void Calc_ReadFirmwareVersion_Response()
    {
        // [2.1] RFID 回版本回包：LENGTH(00 09)+ID(00 21)+STATUS(00)+DATA(00 00 00 01) → sum=0x2B → CS=0xFFD5
        byte[] input = [0x00, 0x09, 0x00, 0x21, 0x00, 0x00, 0x00, 0x00, 0x01];
        Assert.Equal(0xFFD5, Checksum.Calc(input));
    }

    [Fact]
    public void Calc_InventoryData_Packet()
    {
        // [4.1] Inventory Data 回包：21 bytes → sum=0x64D → CS=0xF9B3
        byte[] input =
        [
            0x00, 0x15, 0x00, 0xC0, 0x01, 0xE9, 0x62, 0x34,
            0x00, 0xAA, 0xAA, 0x34, 0x12, 0xDC, 0x03, 0x01,
            0x17, 0x16, 0x10, 0xB9, 0x88
        ];
        Assert.Equal(0xF9B3, Checksum.Calc(input));
    }
}

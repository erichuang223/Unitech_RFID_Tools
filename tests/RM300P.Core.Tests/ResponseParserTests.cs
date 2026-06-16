using RM300P.Core.Models;
using RM300P.Core.Protocol;

namespace RM300P.Core.Tests;

/// <summary>
/// 測試規格 04 階段 -1.4：回包解析正確——各類封包路由到對應事件或同步配對
/// </summary>
public class ResponseParserTests
{
    private static Packet MakePacket(ushort id, byte[] payload)
    {
        byte[] raw = PacketCodec.Encode(id, payload);
        return new Packet(id, payload, raw);
    }

    [Fact]
    public void Handle_SyncResponse_CompletesWaitTask()
    {
        var parser = new ResponseParser();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        Task<Packet> waitTask = parser.WaitForResponseAsync(0x0021, cts.Token);
        parser.HandlePacket(MakePacket(0x0021, [0x00, 0x00, 0x00, 0x00, 0x01]));

        Assert.True(waitTask.IsCompletedSuccessfully);
        Assert.Equal(0x0021, waitTask.Result.Id);
    }

    [Fact]
    public void Handle_InventoryDataPacket_RaisesTagReported()
    {
        var parser = new ResponseParser();
        TagReport? received = null;
        parser.TagReported += (_, e) => received = e;

        // 0xC0 Inventory Data：STATUS(01) ANTID(01) RSSI(FF CE=-50cdBm) PC(10 00) EPC(AA BB CC)
        parser.HandlePacket(MakePacket(0x00C0, [0x01, 0x01, 0xFF, 0xCE, 0x10, 0x00, 0xAA, 0xBB, 0xCC]));

        Assert.NotNull(received);
        Assert.Equal(0x00C0, received!.Id);
        Assert.Equal(1, received.AntId);
    }

    [Fact]
    public void Handle_InventoryDonePacket_RaisesInventoryDone()
    {
        var parser = new ResponseParser();
        bool raised = false;
        parser.InventoryDone += (_, _) => raised = true;

        parser.HandlePacket(MakePacket(0x00C7, [0x00]));

        Assert.True(raised);
    }

    [Fact]
    public void Handle_TagAccessDataPacket_RaisesTagAccessReported()
    {
        var parser = new ResponseParser();
        TagAccessData? received = null;
        parser.TagAccessReported += (_, e) => received = e;

        // 0xC8：STATUS(00) OPID(01) TAERRCODE(00) DATA(DE AD BE EF)
        parser.HandlePacket(MakePacket(0x00C8, [0x00, 0x01, 0x00, 0xDE, 0xAD, 0xBE, 0xEF]));

        Assert.NotNull(received);
        Assert.True(received!.IsSuccess);
        Assert.Equal([0xDE, 0xAD, 0xBE, 0xEF], received.Data);
    }

    [Fact]
    public void Handle_OperationErrorPacket_RaisesErrorReported()
    {
        var parser = new ResponseParser();
        DeviceError? received = null;
        parser.ErrorReported += (_, e) => received = e;

        // 0xD0 Operation Error：STATUS(00) OPERRCODE(01)
        parser.HandlePacket(MakePacket(0x00D0, [0x00, 0x01]));

        Assert.NotNull(received);
        Assert.Equal(DeviceErrorType.Operation, received!.Type);
        Assert.Contains("天線", received.Description);
    }

    [Fact]
    public void Handle_SystemErrorPacket_RaisesErrorReported()
    {
        var parser = new ResponseParser();
        DeviceError? received = null;
        parser.ErrorReported += (_, e) => received = e;

        parser.HandlePacket(MakePacket(0x00D1, [0x00, 0x01]));

        Assert.NotNull(received);
        Assert.Equal(DeviceErrorType.System, received!.Type);
    }

    [Fact]
    public void Handle_AntennaEventPacket_RaisesEventReported()
    {
        var parser = new ResponseParser();
        DeviceEvent? received = null;
        parser.EventReported += (_, e) => received = e;

        parser.HandlePacket(MakePacket(0x00E0, [0x00, 0x00]));

        Assert.NotNull(received);
        Assert.Equal(DeviceEventType.Antenna, received!.Type);
    }

    [Fact]
    public void WaitForResponse_Cancelled_ThrowsTaskCanceledException()
    {
        var parser = new ResponseParser();
        using var cts = new CancellationTokenSource();
        Task<Packet> task = parser.WaitForResponseAsync(0x0021, cts.Token);

        cts.Cancel();

        Assert.True(task.IsCanceled);
    }
}

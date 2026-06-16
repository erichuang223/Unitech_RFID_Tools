using RM300P.Core.Models;

namespace RM300P.Core.Protocol;

/// <summary>
/// 依回包 ID 分三類路由：
///   同步回應（ID 與送出指令相同）→ 配對到等待的請求
///   非同步資料（盤點串流、存取結果）→ 事件
///   錯誤 / 設備事件（主動回報）→ 事件
/// </summary>
public sealed class ResponseParser
{
    // 非同步資料 ID 集合（回包 Chapter 4 / Chapter 7）
    private static readonly HashSet<ushort> AsyncDataIds = new()
    {
        0xC0, 0xC1, 0xC2,          // Inventory Data（基本/TID/相位）
        0xB0, 0xB1, 0xB2,          // Inventory Data（含頻率）
        0xA0,                       // Custom Inventory Data
        0xC7,                       // Inventory Done
        0xC8,                       // Tag Access Data
        0xD0, 0xD1,                 // Operation / System Error
        0xE0, 0xE1                  // Event Notification（天線/LBT）
    };

    public event EventHandler<TagReport>?      TagReported;
    public event EventHandler<InventoryDone>?  InventoryDone;
    public event EventHandler<TagAccessData>?  TagAccessReported;
    public event EventHandler<DeviceError>?    ErrorReported;
    public event EventHandler<DeviceEvent>?    EventReported;

    // 等待同步回應的待辦字典（ID → TaskCompletionSource）
    private readonly Dictionary<ushort, TaskCompletionSource<Packet>> _pending = new();
    private readonly object _lock = new();

    public void HandlePacket(Packet packet)
    {
        if (AsyncDataIds.Contains(packet.Id))
        {
            DispatchAsync(packet);
            return;
        }

        // 嘗試配對同步請求
        lock (_lock)
        {
            if (_pending.TryGetValue(packet.Id, out var tcs))
            {
                _pending.Remove(packet.Id);
                tcs.TrySetResult(packet);
                return;
            }
        }

        // 未知 ID：忽略（可加日誌）
    }

    public Task<Packet> WaitForResponseAsync(ushort id, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<Packet>(TaskCreationOptions.RunContinuationsAsynchronously);
        ct.Register(() => tcs.TrySetCanceled());

        lock (_lock)
            _pending[id] = tcs;

        return tcs.Task;
    }

    private void DispatchAsync(Packet p)
    {
        switch (p.Id)
        {
            case 0xC0 or 0xC1 or 0xC2 or 0xB0 or 0xB1 or 0xB2 or 0xA0:
                TagReported?.Invoke(this, TagReport.Parse(p));
                break;
            case 0xC7:
                InventoryDone?.Invoke(this, new InventoryDone());
                break;
            case 0xC8:
                TagAccessReported?.Invoke(this, TagAccessData.Parse(p));
                break;
            case 0xD0 or 0xD1:
                ErrorReported?.Invoke(this, DeviceError.Parse(p));
                break;
            case 0xE0 or 0xE1:
                EventReported?.Invoke(this, DeviceEvent.Parse(p));
                break;
        }
    }
}

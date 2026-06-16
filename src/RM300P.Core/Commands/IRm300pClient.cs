using RM300P.Core.Models;
using RM300P.Core.Protocol;

namespace RM300P.Core.Commands;

public interface IRm300pClient : IDisposable
{
    // ── 連線 ──────────────────────────────────────────────
    IReadOnlyList<string> GetAvailablePorts();
    void Connect(string portName, int baudRate);
    void Disconnect();
    bool IsConnected { get; }

    // ── 非同步事件（盤點串流 / 設備通知）───────────────────
    event EventHandler<TagReport>     TagReported;
    event EventHandler<InventoryDone> InventoryDone;
    event EventHandler<TagAccessData> TagAccessReported;
    event EventHandler<DeviceError>   ErrorReported;
    event EventHandler<DeviceEvent>   EventReported;
    event EventHandler<byte[]>        RawBytesReceived;

    // ── 裝置資訊（第 2 章）──────────────────────────────────
    Task<CommandResult<string>>  ReadFirmwareVersionAsync(CancellationToken ct = default);
    Task<CommandResult<string>>  ReadModelNameAsync(CancellationToken ct = default);
    Task<CommandResult<string>>  ReadSerialNumberAsync(CancellationToken ct = default);
    Task<CommandResult<ushort>>  ReadSkuIdAsync(CancellationToken ct = default);
    Task<CommandResult<double>>  ReadAmbientTemperatureAsync(CancellationToken ct = default);
    Task<CommandResult<double>>  ReadPaTemperatureAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    ResetDeviceAsync(byte flags = 0, CancellationToken ct = default);

    // ── 盤點控制（第 3 章 + 5.21/5.22）★核心 ────────────────
    Task<CommandResult<byte>>    GetOperationModeAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetOperationModeAsync(byte mode, CancellationToken ct = default);
    Task<CommandResult<bool>>    StartInventoryAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    CancelAsync(CancellationToken ct = default);

    // ── 盤點三模式高階組合 ───────────────────────────────────
    Task SingleInventoryAsync(CancellationToken ct = default);
    Task StartContinuousAsync(CancellationToken ct = default);
    Task StopContinuousAsync(CancellationToken ct = default);
    bool IsInventoryRunning { get; }

    // ── 標籤讀寫（第 3 章）──────────────────────────────────
    Task<CommandResult<bool>>    ReadTagAsync(uint accPw, byte bank, ushort addr, byte len, CancellationToken ct = default);
    Task<CommandResult<bool>>    WriteTagAsync(uint accPw, byte bank, ushort addr, byte[] data, CancellationToken ct = default);

    // ── 參數設定（第 5 章，Get/Set 成對）────────────────────
    Task<CommandResult<byte>>    GetRegionAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetRegionAsync(byte region, CancellationToken ct = default);
    Task<CommandResult<byte[]>>  GetAntennaSettingAsync(byte antId, CancellationToken ct = default);
    Task<CommandResult<bool>>    SetAntennaSettingAsync(byte antId, byte[] settingData, CancellationToken ct = default);
    Task<CommandResult<byte>>    GetAntennaEnabledAsync(byte antId, CancellationToken ct = default);
    Task<CommandResult<bool>>    SetAntennaEnabledAsync(byte antId, bool enabled, CancellationToken ct = default);
    Task<CommandResult<byte>>    GetRfModeAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetRfModeAsync(byte mode, CancellationToken ct = default);
    Task<CommandResult<byte[]>>  GetGen2AlgorithmAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetGen2AlgorithmAsync(byte[] data, CancellationToken ct = default);
    Task<CommandResult<bool>>    GetLbtEnabledAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetLbtEnabledAsync(bool enabled, CancellationToken ct = default);
    Task<CommandResult<bool>>    GetFastIdEnabledAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetFastIdEnabledAsync(bool enabled, CancellationToken ct = default);
    Task<CommandResult<bool>>    GetTagFocusEnabledAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetTagFocusEnabledAsync(bool enabled, CancellationToken ct = default);
    Task<CommandResult<byte[]>>  GetSaveSettingsModeAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetSaveSettingsModeAsync(byte mode, CancellationToken ct = default);
    Task<CommandResult<int>>     GetUartBaudRateAsync(CancellationToken ct = default);
    Task<CommandResult<bool>>    SetUartBaudRateAsync(int baudRate, CancellationToken ct = default);

    // ── 事件通知（第 7 章）──────────────────────────────────
    Task<CommandResult<bool>>    SetEventNotificationAsync(byte eventId, bool enable, CancellationToken ct = default);
}

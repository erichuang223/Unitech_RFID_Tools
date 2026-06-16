using RM300P.Core.Models;
using RM300P.Core.Protocol;
using RM300P.Core.Transport;

namespace RM300P.Core.Commands;

public sealed class Rm300pClient : IRm300pClient
{
    private readonly ISerialTransport _transport;
    private readonly ResponseParser   _parser;
    private readonly TimeSpan         _timeout;
    private bool _inventoryRunning;

    public bool IsConnected      => _transport.IsOpen;
    public bool IsInventoryRunning => _inventoryRunning;

    public event EventHandler<TagReport>?     TagReported;
    public event EventHandler<InventoryDone>? InventoryDone;
    public event EventHandler<TagAccessData>? TagAccessReported;
    public event EventHandler<DeviceError>?   ErrorReported;
    public event EventHandler<DeviceEvent>?   EventReported;
    public event EventHandler<byte[]>?        RawBytesReceived;

    public Rm300pClient(ISerialTransport? transport = null, int timeoutMs = 1000)
    {
        _transport = transport ?? new SerialPortTransport();
        _timeout   = TimeSpan.FromMilliseconds(timeoutMs);
        _parser    = new ResponseParser();

        _transport.PacketReceived += (_, pkt) => _parser.HandlePacket(pkt);
        _transport.RawReceived    += (_, raw) => RawBytesReceived?.Invoke(this, raw);

        _parser.TagReported       += (_, e) => TagReported?.Invoke(this, e);
        _parser.InventoryDone     += (_, e) => { _inventoryRunning = false; InventoryDone?.Invoke(this, e); };
        _parser.TagAccessReported += (_, e) => TagAccessReported?.Invoke(this, e);
        _parser.ErrorReported     += (_, e) => ErrorReported?.Invoke(this, e);
        _parser.EventReported     += (_, e) => EventReported?.Invoke(this, e);
    }

    // ── 連線 ──────────────────────────────────────────────

    public IReadOnlyList<string> GetAvailablePorts() => _transport.GetAvailablePorts();
    public void Connect(string portName, int baudRate) => _transport.Open(portName, baudRate);
    public void Disconnect() => _transport.Close();

    // ── 低階收發 ───────────────────────────────────────────

    private async Task<Packet> SendAndWaitAsync(ushort id, byte[]? data = null, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(_timeout);

        Task<Packet> waitTask = _parser.WaitForResponseAsync(id, cts.Token);
        _transport.SendRaw(data is null ? PacketCodec.Encode(id) : PacketCodec.Encode(id, data));
        return await waitTask.ConfigureAwait(false);
    }

    private static StatusCode ParseStatus(Packet p) =>
        p.Payload.Length > 0 ? (StatusCode)p.Payload[0] : StatusCode.Unknown;

    // ── 裝置資訊（第 2 章）──────────────────────────────────

    public async Task<CommandResult<string>> ReadFirmwareVersionAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0021, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<string>.Fail(s, p);
        // DATA: MAJOR(1) MINOR(1) BUILD(1) REVISION(2,BE)
        string ver = p.Payload.Length >= 5
            ? $"{p.Payload[1]}.{p.Payload[2]}.{p.Payload[3]}.{(p.Payload[4] << 8) | p.Payload[5]}"
            : "未知";
        return CommandResult<string>.Ok(ver, p);
    }

    public async Task<CommandResult<string>> ReadModelNameAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0022, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<string>.Fail(s, p);
        string name = System.Text.Encoding.ASCII.GetString(p.Payload[1..]).TrimEnd('\0');
        return CommandResult<string>.Ok(name, p);
    }

    public async Task<CommandResult<string>> ReadSerialNumberAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0024, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<string>.Fail(s, p);
        string sn = System.Text.Encoding.ASCII.GetString(p.Payload[1..]).TrimEnd('\0');
        return CommandResult<string>.Ok(sn, p);
    }

    public async Task<CommandResult<ushort>> ReadSkuIdAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0025, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<ushort>.Fail(s, p);
        ushort sku = (ushort)((p.Payload[1] << 8) | p.Payload[2]);
        return CommandResult<ushort>.Ok(sku, p);
    }

    public async Task<CommandResult<double>> ReadAmbientTemperatureAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0028, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<double>.Fail(s, p);
        short raw = (short)((p.Payload[1] << 8) | p.Payload[2]);
        return CommandResult<double>.Ok(raw / 10.0, p);
    }

    public async Task<CommandResult<double>> ReadPaTemperatureAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0029, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<double>.Fail(s, p);
        short raw = (short)((p.Payload[1] << 8) | p.Payload[2]);
        return CommandResult<double>.Ok(raw / 10.0, p);
    }

    public async Task<CommandResult<bool>> ResetDeviceAsync(byte flags = 0, CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x002F, [flags], ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    // ── 盤點控制（第 3 章）──────────────────────────────────

    public async Task<CommandResult<byte>> GetOperationModeAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0116, null, ct);
        var s = ParseStatus(p);
        if (s != StatusCode.Success) return CommandResult<byte>.Fail(s, p);
        return CommandResult<byte>.Ok(p.Payload[1], p);
    }

    public async Task<CommandResult<bool>> SetOperationModeAsync(byte mode, CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0117, [mode], ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    public async Task<CommandResult<bool>> StartInventoryAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0080, null, ct);
        var s = ParseStatus(p);
        if (s == StatusCode.Success) _inventoryRunning = true;
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    public async Task<CommandResult<bool>> CancelAsync(CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(0x0081, null, ct);
        var s = ParseStatus(p);
        _inventoryRunning = false;
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    // ── 盤點三模式高階組合 ───────────────────────────────────

    public async Task SingleInventoryAsync(CancellationToken ct = default)
    {
        await SetOperationModeAsync(1, ct);   // 1 = 非連續
        await StartInventoryAsync(ct);
        // InventoryDone 事件由 ResponseParser 在收到 0xC7 時觸發，_inventoryRunning 會自動歸 false
    }

    public async Task StartContinuousAsync(CancellationToken ct = default)
    {
        await SetOperationModeAsync(0, ct);   // 0 = 連續
        await StartInventoryAsync(ct);
    }

    public async Task StopContinuousAsync(CancellationToken ct = default) =>
        await CancelAsync(ct);

    // ── 標籤讀寫（第 3 章）──────────────────────────────────

    public async Task<CommandResult<bool>> ReadTagAsync(uint accPw, byte bank, ushort addr, byte len, CancellationToken ct = default)
    {
        byte[] data =
        [
            (byte)(accPw >> 24), (byte)(accPw >> 16), (byte)(accPw >> 8), (byte)accPw,
            bank,
            (byte)(addr >> 8), (byte)addr,
            len
        ];
        var p = await SendAndWaitAsync(0x0082, data, ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    public async Task<CommandResult<bool>> WriteTagAsync(uint accPw, byte bank, ushort addr, byte[] writeData, CancellationToken ct = default)
    {
        byte[] header =
        [
            (byte)(accPw >> 24), (byte)(accPw >> 16), (byte)(accPw >> 8), (byte)accPw,
            bank,
            (byte)(addr >> 8), (byte)addr
        ];
        byte[] data = [..header, ..writeData];
        var p = await SendAndWaitAsync(0x0083, data, ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    // ── 參數設定（第 5 章）──────────────────────────────────

    private async Task<CommandResult<byte[]>> GetRawAsync(ushort id, byte[]? reqData = null, CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(id, reqData, ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success
            ? CommandResult<byte[]>.Ok(p.Payload[1..], p)
            : CommandResult<byte[]>.Fail(s, p);
    }

    private async Task<CommandResult<bool>> SetRawAsync(ushort id, byte[] data, CancellationToken ct = default)
    {
        var p = await SendAndWaitAsync(id, data, ct);
        var s = ParseStatus(p);
        return s == StatusCode.Success ? CommandResult<bool>.Ok(true, p) : CommandResult<bool>.Fail(s, p);
    }

    public Task<CommandResult<byte>> GetRegionAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0100, null, ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<byte>.Ok(t.Result.Data![0], t.Result.Raw)
                : CommandResult<byte>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetRegionAsync(byte region, CancellationToken ct = default) =>
        SetRawAsync(0x0101, [region], ct);

    public Task<CommandResult<byte[]>> GetAntennaSettingAsync(byte antId, CancellationToken ct = default) =>
        GetRawAsync(0x0102, [antId], ct);

    public Task<CommandResult<bool>> SetAntennaSettingAsync(byte antId, byte[] settingData, CancellationToken ct = default) =>
        SetRawAsync(0x0103, [antId, ..settingData], ct);

    public Task<CommandResult<byte>> GetAntennaEnabledAsync(byte antId, CancellationToken ct = default) =>
        GetRawAsync(0x0104, [antId], ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<byte>.Ok(t.Result.Data![0], t.Result.Raw)
                : CommandResult<byte>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetAntennaEnabledAsync(byte antId, bool enabled, CancellationToken ct = default) =>
        SetRawAsync(0x0105, [antId, (byte)(enabled ? 1 : 0)], ct);

    public Task<CommandResult<byte>> GetRfModeAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0106, null, ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<byte>.Ok(t.Result.Data![0], t.Result.Raw)
                : CommandResult<byte>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetRfModeAsync(byte mode, CancellationToken ct = default) =>
        SetRawAsync(0x0107, [mode], ct);

    public Task<CommandResult<byte[]>> GetGen2AlgorithmAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0108, null, ct);

    public Task<CommandResult<bool>> SetGen2AlgorithmAsync(byte[] data, CancellationToken ct = default) =>
        SetRawAsync(0x0109, data, ct);

    public Task<CommandResult<bool>> GetLbtEnabledAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0110, null, ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<bool>.Ok(t.Result.Data![0] != 0, t.Result.Raw)
                : CommandResult<bool>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetLbtEnabledAsync(bool enabled, CancellationToken ct = default) =>
        SetRawAsync(0x0111, [(byte)(enabled ? 1 : 0)], ct);

    public Task<CommandResult<bool>> GetFastIdEnabledAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0112, null, ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<bool>.Ok(t.Result.Data![0] != 0, t.Result.Raw)
                : CommandResult<bool>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetFastIdEnabledAsync(bool enabled, CancellationToken ct = default) =>
        SetRawAsync(0x0113, [(byte)(enabled ? 1 : 0)], ct);

    public Task<CommandResult<bool>> GetTagFocusEnabledAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0114, null, ct).ContinueWith(t =>
            t.Result.IsSuccess
                ? CommandResult<bool>.Ok(t.Result.Data![0] != 0, t.Result.Raw)
                : CommandResult<bool>.Fail(t.Result.Status, t.Result.Raw));

    public Task<CommandResult<bool>> SetTagFocusEnabledAsync(bool enabled, CancellationToken ct = default) =>
        SetRawAsync(0x0115, [(byte)(enabled ? 1 : 0)], ct);

    public Task<CommandResult<byte[]>> GetSaveSettingsModeAsync(CancellationToken ct = default) =>
        GetRawAsync(0x01F0, null, ct);

    public Task<CommandResult<bool>> SetSaveSettingsModeAsync(byte mode, CancellationToken ct = default) =>
        SetRawAsync(0x01F1, [mode], ct);

    public Task<CommandResult<int>> GetUartBaudRateAsync(CancellationToken ct = default) =>
        GetRawAsync(0x0150, null, ct).ContinueWith(t =>
        {
            if (!t.Result.IsSuccess) return CommandResult<int>.Fail(t.Result.Status, t.Result.Raw);
            var d = t.Result.Data!;
            int baud = d.Length >= 4 ? (d[0] << 24) | (d[1] << 16) | (d[2] << 8) | d[3] : 0;
            return CommandResult<int>.Ok(baud, t.Result.Raw);
        });

    public Task<CommandResult<bool>> SetUartBaudRateAsync(int baudRate, CancellationToken ct = default) =>
        SetRawAsync(0x0151,
        [
            (byte)(baudRate >> 24), (byte)(baudRate >> 16),
            (byte)(baudRate >> 8),  (byte)baudRate
        ], ct);

    // ── 事件通知（第 7 章）──────────────────────────────────

    public Task<CommandResult<bool>> SetEventNotificationAsync(byte eventId, bool enable, CancellationToken ct = default) =>
        SetRawAsync(0x0090, [eventId, (byte)(enable ? 1 : 0)], ct);

    public void Dispose() => _transport.Dispose();
}

using RM300P.Core.Commands;
using RM300P.Core.Models;
using RM300P.Core.Protocol;
using Spectre.Console;

namespace RM300P.ConsoleTool.Menu;

/// <summary>
/// 主選單驅動器。依規格書 03 §3 選單樹實作光棒導覽。
/// </summary>
public sealed class MenuRunner
{
    private readonly IRm300pClient _client;
    private string _lastResult = string.Empty;
    private string _lastTx     = string.Empty;
    private string _lastRx     = string.Empty;

    public MenuRunner(IRm300pClient client)
    {
        _client = client;
        _client.RawBytesReceived += (_, raw) =>
            _lastRx = "[收] " + BitConverter.ToString(raw).Replace("-", " ");
    }

    public async Task RunAsync()
    {
        AnsiConsole.Clear();
        while (true)
        {
            RenderHeader();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]主選單[/]")
                    .HighlightStyle(new Style(foreground: Color.Black, background: Color.Cyan1))
                    .AddChoices(
                        "連線 (Connection)",
                        "盤點收集 (Inventory) ★",
                        "裝置資訊 (Device Info)",
                        "標籤讀寫 (Tag R/W)",
                        "參數設定 (Configuration)",
                        "事件通知 (Events)",
                        "診斷監看 (Diagnostics)",
                        "── 離開 ──"));

            if (choice.StartsWith("──")) break;

            await HandleMainMenuAsync(choice);
        }
    }

    // ── 標題列 ─────────────────────────────────────────────

    private void RenderHeader()
    {
        AnsiConsole.Clear();
        string connStatus = _client.IsConnected
            ? "[green]● 已連線[/]"
            : "[red]○ 未連線[/]";

        AnsiConsole.MarkupLine($"[bold]RM300P Bench Tool[/]  {connStatus}");
        AnsiConsole.Write(new Rule());
        if (!string.IsNullOrEmpty(_lastResult))
            AnsiConsole.MarkupLine($"[dim]{Markup.Escape(_lastResult)}[/]");
        if (!string.IsNullOrEmpty(_lastTx))
            AnsiConsole.MarkupLine($"[dim]{Markup.Escape(_lastTx)}[/]");
        if (!string.IsNullOrEmpty(_lastRx))
            AnsiConsole.MarkupLine($"[dim]{Markup.Escape(_lastRx)}[/]");
        AnsiConsole.Write(new Rule());
        AnsiConsole.MarkupLine("[dim]↑↓ 移動  Enter 選擇  Esc 返回[/]");
        Console.WriteLine();
    }

    // ── 主選單分派 ─────────────────────────────────────────

    private async Task HandleMainMenuAsync(string choice)
    {
        if      (choice.StartsWith("連線"))     await ConnectionMenuAsync();
        else if (choice.StartsWith("盤點"))     await InventoryMenuAsync();
        else if (choice.StartsWith("裝置"))     await DeviceInfoMenuAsync();
        else if (choice.StartsWith("標籤"))     await TagRwMenuAsync();
        else if (choice.StartsWith("參數"))     await ConfigMenuAsync();
        else if (choice.StartsWith("事件"))     await EventMenuAsync();
        else if (choice.StartsWith("診斷"))     DiagnosticsMenu();
    }

    // ── 連線 ───────────────────────────────────────────────

    private async Task ConnectionMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]連線[/]")
                .AddChoices("選擇 COM Port 並連線", "斷線", "← 返回"));

        if (choice.StartsWith("選擇"))
        {
            var ports = _client.GetAvailablePorts();
            if (ports.Count == 0) { SetResult("找不到任何 COM port。"); return; }

            string port = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("選擇 COM Port").AddChoices(ports));

            string baudStr = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Baud Rate")
                    .AddChoices("115200", "460800", "921600"));

            int baud = int.Parse(baudStr);
            try
            {
                _client.Connect(port, baud);
                SetResult($"已連線：{port} @ {baud}");
            }
            catch (Exception ex)
            {
                SetResult($"連線失敗：{ex.Message}");
            }
        }
        else if (choice.StartsWith("斷線"))
        {
            _client.Disconnect();
            SetResult("已斷線。");
        }

        await Task.CompletedTask;
    }

    // ── 盤點收集 ────────────────────────────────────────────

    private async Task InventoryMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]盤點收集[/] ★")
                .AddChoices(
                    "盤點模式設定",
                    "單次盤點收集",
                    "開啟連續收集",
                    "停止連續收集",
                    "← 返回"));

        if (!RequireConnected()) return;

        if (choice == "盤點模式設定")
        {
            string mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("盤點模式")
                    .AddChoices("非連續（單次）", "連續"));
            byte modeVal = mode.StartsWith("連續") ? (byte)0 : (byte)1;
            var r = await _client.SetOperationModeAsync(modeVal);
            SetResult(r.IsSuccess ? $"模式設定成功：{mode}" : $"失敗：{r.Status.ToChineseDescription()}");
            LogTx(PacketCodec.Encode(0x0117, [modeVal]));
        }
        else if (choice == "單次盤點收集")
        {
            await Views.InventoryView.RunAsync(_client, continuous: false);
        }
        else if (choice == "開啟連續收集")
        {
            await Views.InventoryView.RunAsync(_client, continuous: true);
        }
        else if (choice == "停止連續收集")
        {
            var r = await _client.CancelAsync();
            SetResult(r.IsSuccess ? "已停止連續收集。" : $"失敗：{r.Status.ToChineseDescription()}");
        }
    }

    // ── 裝置資訊 ────────────────────────────────────────────

    private async Task DeviceInfoMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]裝置資訊[/]")
                .AddChoices("韌體版本", "機型", "序號", "SKU",
                            "環境溫度", "PA 溫度", "重置裝置", "← 返回"));

        if (!RequireConnected()) return;

        switch (choice)
        {
            case "韌體版本":
            {
                var r = await _client.ReadFirmwareVersionAsync();
                SetResult(r.IsSuccess ? $"韌體版本：{r.Data}" : $"失敗：{r.Status.ToChineseDescription()}");
                if (r.Raw != null) LogRx(r.Raw.Raw);
                break;
            }
            case "機型":
            {
                var r = await _client.ReadModelNameAsync();
                SetResult(r.IsSuccess ? $"機型：{r.Data}" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
            case "序號":
            {
                var r = await _client.ReadSerialNumberAsync();
                SetResult(r.IsSuccess ? $"序號：{r.Data}" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
            case "SKU":
            {
                var r = await _client.ReadSkuIdAsync();
                SetResult(r.IsSuccess ? $"SKU：0x{r.Data:X4}" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
            case "環境溫度":
            {
                var r = await _client.ReadAmbientTemperatureAsync();
                SetResult(r.IsSuccess ? $"環境溫度：{r.Data:F1} °C" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
            case "PA 溫度":
            {
                var r = await _client.ReadPaTemperatureAsync();
                SetResult(r.IsSuccess ? $"PA 溫度：{r.Data:F1} °C" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
            case "重置裝置":
            {
                if (!AnsiConsole.Confirm("[red]確定要重置裝置？[/]")) break;
                bool restore = AnsiConsole.Confirm("同時恢復原廠設定？");
                if (restore && !AnsiConsole.Confirm("[red]再次確認：恢復原廠將清除所有設定，繼續？[/]")) break;
                var r = await _client.ResetDeviceAsync(restore ? (byte)1 : (byte)0);
                SetResult(r.IsSuccess ? "重置指令已送出。" : $"失敗：{r.Status.ToChineseDescription()}");
                break;
            }
        }
    }

    // ── 標籤讀寫 ────────────────────────────────────────────

    private async Task TagRwMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]標籤讀寫[/]")
                .AddChoices("讀取標籤", "寫入標籤（限測試標籤）", "← 返回"));

        if (!RequireConnected()) return;

        if (choice == "讀取標籤")
        {
            uint  accPw = ReadHexUint("Access Password（hex，如 00000000）");
            byte  bank  = ReadByte("Bank（0=Reserved 1=EPC 2=TID 3=USER）");
            ushort addr = ReadUshort("起始位址（Word address）");
            byte  len   = ReadByte("讀取長度（Word count）");
            var r = await _client.ReadTagAsync(accPw, bank, addr, len);
            SetResult(r.IsSuccess ? "讀取指令已送出，等待 TagAccessReported 事件。" : $"失敗：{r.Status.ToChineseDescription()}");
        }
        else if (choice.StartsWith("寫入"))
        {
            AnsiConsole.MarkupLine("[red]警告：僅用於測試標籤！[/]");
            if (!AnsiConsole.Confirm("確認此為測試標籤，繼續？")) return;
            uint   accPw = ReadHexUint("Access Password");
            byte   bank  = ReadByte("Bank");
            ushort addr  = ReadUshort("起始位址");
            string hex   = AnsiConsole.Ask<string>("寫入資料（hex，不含空格，如 DEADBEEF）");
            byte[] data  = Convert.FromHexString(hex);
            if (!AnsiConsole.Confirm($"確認寫入 {hex} 到 Bank={bank} Addr={addr}？")) return;
            var r = await _client.WriteTagAsync(accPw, bank, addr, data);
            SetResult(r.IsSuccess ? "寫入指令已送出。" : $"失敗：{r.Status.ToChineseDescription()}");
        }
    }

    // ── 參數設定 ────────────────────────────────────────────

    private async Task ConfigMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]參數設定[/]")
                .AddChoices(
                    "區域 Region",
                    "天線啟用狀態",
                    "RF Mode",
                    "LBT 開關",
                    "FastID",
                    "TagFocus",
                    "UART 鮑率（⚠ 改錯將失聯）",
                    "設定儲存模式",
                    "← 返回"));

        if (!RequireConnected()) return;

        switch (choice)
        {
            case "區域 Region":
            {
                var g = await _client.GetRegionAsync();
                if (!g.IsSuccess) { SetResult($"Get 失敗：{g.Status.ToChineseDescription()}"); break; }
                AnsiConsole.MarkupLine($"目前區域：[cyan]{g.Data}[/]（0=FCC 1=ETSI 3=TAIWAN…）");
                if (!AnsiConsole.Confirm("要修改嗎？")) break;
                byte val = ReadByte("新區域值");
                var s = await _client.SetRegionAsync(val);
                SetResult(s.IsSuccess ? $"區域已設為 {val}" : $"Set 失敗：{s.Status.ToChineseDescription()}");
                break;
            }
            case "天線啟用狀態":
            {
                byte antId = ReadByte("天線 ID（1~4）");
                var g = await _client.GetAntennaEnabledAsync(antId);
                if (!g.IsSuccess) { SetResult($"Get 失敗：{g.Status.ToChineseDescription()}"); break; }
                AnsiConsole.MarkupLine($"天線 {antId} 目前：[cyan]{(g.Data != 0 ? "啟用" : "停用")}[/]");
                if (!AnsiConsole.Confirm("要修改嗎？")) break;
                bool en = AnsiConsole.Confirm("啟用？");
                var s = await _client.SetAntennaEnabledAsync(antId, en);
                SetResult(s.IsSuccess ? $"天線 {antId} 已{(en ? "啟用" : "停用")}" : $"Set 失敗：{s.Status.ToChineseDescription()}");
                break;
            }
            case "UART 鮑率（⚠ 改錯將失聯）":
            {
                AnsiConsole.MarkupLine("[red]警告：改錯將導致失聯，需重開機生效，請記住新值！[/]");
                var g = await _client.GetUartBaudRateAsync();
                if (g.IsSuccess) AnsiConsole.MarkupLine($"目前鮑率：[cyan]{g.Data}[/]");
                if (!AnsiConsole.Confirm("確定要修改？")) break;
                string baudStr = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("新鮑率")
                        .AddChoices("115200", "460800", "921600"));
                int baud = int.Parse(baudStr);
                if (!AnsiConsole.Confirm($"確認改為 {baud}（重開機後生效）？")) break;
                var s = await _client.SetUartBaudRateAsync(baud);
                SetResult(s.IsSuccess ? $"鮑率設為 {baud}，請重開機後以新鮑率重新連線。" : $"Set 失敗：{s.Status.ToChineseDescription()}");
                break;
            }
            case "設定儲存模式":
            {
                AnsiConsole.MarkupLine("[yellow]測試期建議使用 RAM-only，避免誤寫 Flash。[/]");
                var s = await _client.SetSaveSettingsModeAsync(0);   // 0 = RAM-only
                SetResult(s.IsSuccess ? "已設為 RAM-only 儲存模式。" : $"失敗：{s.Status.ToChineseDescription()}");
                break;
            }
            default:
                SetResult($"（{choice} 尚未在此版本實作）");
                break;
        }
    }

    // ── 事件通知 ────────────────────────────────────────────

    private async Task EventMenuAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]事件通知[/]")
                .AddChoices(
                    "啟用 天線狀態事件 (0xE0)",
                    "停用 天線狀態事件 (0xE0)",
                    "啟用 LBT 狀態事件 (0xE1)",
                    "停用 LBT 狀態事件 (0xE1)",
                    "← 返回"));

        if (!RequireConnected()) return;

        byte id;
        bool en;
        switch (choice)
        {
            case "啟用 天線狀態事件 (0xE0)": id = 0xE0; en = true;  break;
            case "停用 天線狀態事件 (0xE0)": id = 0xE0; en = false; break;
            case "啟用 LBT 狀態事件 (0xE1)":  id = 0xE1; en = true;  break;
            case "停用 LBT 狀態事件 (0xE1)":  id = 0xE1; en = false; break;
            default: return;
        }

        var r = await _client.SetEventNotificationAsync(id, en);
        SetResult(r.IsSuccess ? $"0x{id:X2} 事件已{(en ? "啟用" : "停用")}。" : $"失敗：{r.Status.ToChineseDescription()}");
    }

    // ── 診斷監看 ────────────────────────────────────────────

    private void DiagnosticsMenu()
    {
        AnsiConsole.MarkupLine("[bold]診斷監看[/]");
        AnsiConsole.MarkupLine($"最後傳送：[dim]{Markup.Escape(_lastTx)}[/]");
        AnsiConsole.MarkupLine($"最後接收：[dim]{Markup.Escape(_lastRx)}[/]");
        AnsiConsole.MarkupLine("[dim]按任意鍵返回…[/]");
        Console.ReadKey(true);
    }

    // ── 輔助 ────────────────────────────────────────────────

    private bool RequireConnected()
    {
        if (_client.IsConnected) return true;
        SetResult("[red]請先連線。[/]");
        AnsiConsole.MarkupLine("[red]請先到「連線」選單建立連線。[/]");
        AnsiConsole.MarkupLine("[dim]按任意鍵返回…[/]");
        Console.ReadKey(true);
        return false;
    }

    private void SetResult(string msg)  => _lastResult = "[結果] " + msg;
    private void LogTx(byte[] bytes)    => _lastTx = "[送] " + BitConverter.ToString(bytes).Replace("-", " ");
    private void LogRx(byte[] bytes)    => _lastRx = "[收] " + BitConverter.ToString(bytes).Replace("-", " ");

    private static byte   ReadByte(string prompt)   => (byte)int.Parse(AnsiConsole.Ask<string>(prompt));
    private static ushort ReadUshort(string prompt)  => ushort.Parse(AnsiConsole.Ask<string>(prompt));
    private static uint   ReadHexUint(string prompt) => Convert.ToUInt32(AnsiConsole.Ask<string>(prompt), 16);
}

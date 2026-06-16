using RM300P.Core.Commands;
using RM300P.Core.Models;
using Spectre.Console;

namespace RM300P.ConsoleTool.Views;

/// <summary>
/// 盤點即時表格檢視（規格書 03 §4）。
/// continuous=true → 連續模式；false → 單次模式（收到 InventoryDone 後自動停）。
/// </summary>
public static class InventoryView
{
    public static async Task RunAsync(IRm300pClient client, bool continuous)
    {
        var tagMap  = new Dictionary<string, TagEntry>();
        int total   = 0;
        bool done   = false;
        var lockObj = new object();

        void OnTag(object? _, TagReport r)
        {
            lock (lockObj)
            {
                total++;
                string key = r.EpcHex;
                if (!tagMap.TryGetValue(key, out var entry))
                    entry = new TagEntry { Epc = key, AntId = r.AntId };
                entry.Count++;
                entry.Rssi     = r.Rssi;
                entry.LastSeen = DateTime.Now;
                tagMap[key]    = entry;
            }
        }

        void OnDone(object? _, InventoryDone _2) => done = true;

        client.TagReported    += OnTag;
        client.InventoryDone  += OnDone;

        try
        {
            if (continuous)
                await client.StartContinuousAsync();
            else
                await client.SingleInventoryAsync();

            string modeLabel = continuous ? "連續" : "單次";
            using var cts = new CancellationTokenSource();

            await AnsiConsole.Live(BuildTable(tagMap, total, modeLabel, continuous ? "進行中" : "進行中"))
                .StartAsync(async ctx =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Escape)
                            {
                                cts.Cancel();
                                break;
                            }
                        }

                        if (!continuous && done)
                        {
                            ctx.UpdateTarget(BuildTable(tagMap, total, modeLabel, "本輪完成"));
                            await Task.Delay(800);
                            break;
                        }

                        lock (lockObj)
                            ctx.UpdateTarget(BuildTable(tagMap, total, modeLabel,
                                continuous ? "進行中" : "進行中"));

                        await Task.Delay(200);
                    }
                });

            if (continuous && !done)
                await client.StopContinuousAsync();
        }
        finally
        {
            client.TagReported   -= OnTag;
            client.InventoryDone -= OnDone;
        }
    }

    private static Table BuildTable(
        Dictionary<string, TagEntry> tagMap,
        int total, string mode, string status)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .Title($"盤點 {status}  模式:{mode}  累計:{total} 筆 / 唯一:{tagMap.Count}  [Esc 停止]")
            .AddColumn("ANT")
            .AddColumn("EPC")
            .AddColumn("RSSI")
            .AddColumn("次數")
            .AddColumn("末次");

        foreach (var e in tagMap.Values.OrderByDescending(x => x.Count).Take(30))
        {
            table.AddRow(
                e.AntId.ToString(),
                e.Epc,
                e.Rssi.ToString(),
                e.Count.ToString(),
                e.LastSeen.ToString("HH:mm:ss"));
        }

        return table;
    }

    private sealed class TagEntry
    {
        public string   Epc      { get; set; } = string.Empty;
        public byte     AntId    { get; set; }
        public short    Rssi     { get; set; }
        public int      Count    { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.Now;
    }
}

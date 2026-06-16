using RM300P.Core.Protocol;

namespace RM300P.Core.Models;

public enum DeviceEventType { Antenna, Lbt }

public sealed class DeviceEvent
{
    public DeviceEventType Type        { get; init; }
    public byte            EventData   { get; init; }
    public string          Description { get; init; } = string.Empty;

    public static DeviceEvent Parse(Packet p)
    {
        byte data = p.Payload.Length > 1 ? p.Payload[1] : (byte)0;
        var  type = p.Id == 0xE0 ? DeviceEventType.Antenna : DeviceEventType.Lbt;

        string desc = type == DeviceEventType.Antenna
            ? (data == 0 ? "天線事件：開始" : "天線事件：結束")
            : $"LBT 狀態事件：0x{data:X2}";

        return new DeviceEvent { Type = type, EventData = data, Description = desc };
    }
}

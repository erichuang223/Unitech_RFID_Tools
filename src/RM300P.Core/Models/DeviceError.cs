using RM300P.Core.Protocol;

namespace RM300P.Core.Models;

public enum DeviceErrorType { Operation, System }

public sealed class DeviceError
{
    public DeviceErrorType Type      { get; init; }
    public byte            ErrorCode { get; init; }
    public string          Description { get; init; } = string.Empty;

    public static DeviceError Parse(Packet p)
    {
        byte code = p.Payload.Length > 1 ? p.Payload[1] : (byte)0xFF;
        var type  = p.Id == 0xD0 ? DeviceErrorType.Operation : DeviceErrorType.System;

        string desc = type == DeviceErrorType.Operation
            ? DescribeOpError(code)
            : DescribeSysError(code);

        return new DeviceError { Type = type, ErrorCode = code, Description = desc };
    }

    private static string DescribeOpError(byte code) => code switch
    {
        0x01 => "未啟用任何天線",
        0x02 => "RF 載波偵測失敗（LBT）",
        0x03 => "天線功率異常",
        _    => $"操作錯誤 0x{code:X2}"
    };

    private static string DescribeSysError(byte code) => code switch
    {
        0x01 => "溫度過高，已停止 RF",
        0x02 => "系統內部錯誤",
        _    => $"系統錯誤 0x{code:X2}"
    };
}

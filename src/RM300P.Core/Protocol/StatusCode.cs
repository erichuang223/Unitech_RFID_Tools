namespace RM300P.Core.Protocol;

public enum StatusCode : byte
{
    Success          = 0x00,
    Busy             = 0x01,
    InvalidParameter = 0x02,
    NotSupported     = 0x03,
    Timeout          = 0x04,
    Unknown          = 0xFF
}

public static class StatusCodeExtensions
{
    public static string ToChineseDescription(this StatusCode code) => code switch
    {
        StatusCode.Success          => "成功",
        StatusCode.Busy             => "忙碌中",
        StatusCode.InvalidParameter => "參數無效",
        StatusCode.NotSupported     => "不支援",
        StatusCode.Timeout          => "逾時",
        _                           => $"未知狀態碼 0x{(byte)code:X2}"
    };
}

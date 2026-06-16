using RM300P.Core.Protocol;

namespace RM300P.Core.Transport;

/// <summary>
/// 從連續 byte stream 切分出完整封包。
/// 不可用「掃到 ETX(0x03) 就切包」——DATA 裡可能含 0x02/0x03，必須靠 LENGTH 欄位決定長度。
/// 總長 = STX(1) + LENGTH(2) + [ID+STATUS+DATA](LENGTH-2) + CHECKSUM(2) + ETX(1) = LENGTH + 4
/// </summary>
public sealed class PacketFramer
{
    private const byte Stx = 0x02;
    private const byte Etx = 0x03;

    private readonly List<byte> _buffer = new();
    private readonly Action<Packet> _onPacket;
    private readonly Action<string>? _onError;

    public PacketFramer(Action<Packet> onPacket, Action<string>? onError = null)
    {
        _onPacket = onPacket;
        _onError = onError;
    }

    public void Feed(byte[] data) => Feed(data.AsSpan());

    public void Feed(ReadOnlySpan<byte> data)
    {
        foreach (byte b in data)
            _buffer.Add(b);

        TryExtractPackets();
    }

    private void TryExtractPackets()
    {
        while (_buffer.Count > 0)
        {
            // 找 STX
            int stxIndex = _buffer.IndexOf(Stx);
            if (stxIndex < 0)
            {
                _buffer.Clear();
                return;
            }

            if (stxIndex > 0)
                _buffer.RemoveRange(0, stxIndex);   // 丟棄 STX 之前的雜訊

            // 需要至少 STX(1) + LENGTH(2) = 3 bytes 才能知道封包長度
            if (_buffer.Count < 3)
                return;

            int lengthValue = (_buffer[1] << 8) | _buffer[2];   // big-endian
            int totalLength = lengthValue + 4;                   // STX + LENGTH + payload + CHECKSUM + ETX

            if (_buffer.Count < totalLength)
                return;     // 資料不足，等更多

            // 取出整包
            byte[] raw = _buffer.GetRange(0, totalLength).ToArray();
            _buffer.RemoveRange(0, totalLength);

            // 驗證 ETX
            if (raw[^1] != Etx)
            {
                _onError?.Invoke($"ETX 錯誤：期望 0x03，實際 0x{raw[^1]:X2}");
                continue;
            }

            // 驗證 CHECKSUM（輸入區 = LENGTH 到 DATA 結束，共 lengthValue 個 bytes，從索引 1 開始）
            var checksumInput = raw.AsSpan(1, lengthValue);
            ushort expected = Checksum.Calc(checksumInput);
            ushort actual = (ushort)((raw[^3] << 8) | raw[^2]);

            if (expected != actual)
            {
                _onError?.Invoke($"CHECKSUM 錯誤：期望 0x{expected:X4}，實際 0x{actual:X4}");
                continue;
            }

            // 解出 ID 與 Payload（STATUS + DATA）
            ushort id = (ushort)((raw[3] << 8) | raw[4]);
            byte[] payload = raw[5..^3];   // 跳過 STX(1)+LENGTH(2)+ID(2)，取到 CHECKSUM 前

            _onPacket(new Packet(id, payload, raw));
        }
    }
}

using RM300P.Core.Protocol;

namespace RM300P.Core.Transport;

public interface ISerialTransport : IDisposable
{
    IReadOnlyList<string> GetAvailablePorts();
    void Open(string portName, int baudRate);
    void Close();
    bool IsOpen { get; }

    void SendRaw(byte[] bytes);
    event EventHandler<byte[]> RawReceived;
    event EventHandler<Packet> PacketReceived;
}

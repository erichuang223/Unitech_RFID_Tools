using System.IO.Ports;
using RM300P.Core.Protocol;

namespace RM300P.Core.Transport;

public sealed class SerialPortTransport : ISerialTransport
{
    private SerialPort? _port;
    private PacketFramer? _framer;

    public bool IsOpen => _port?.IsOpen ?? false;

    public event EventHandler<byte[]>? RawReceived;
    public event EventHandler<Packet>? PacketReceived;

    public IReadOnlyList<string> GetAvailablePorts() =>
        SerialPort.GetPortNames();

    public void Open(string portName, int baudRate)
    {
        if (_port?.IsOpen == true)
            Close();

        _framer = new PacketFramer(
            onPacket: pkt => PacketReceived?.Invoke(this, pkt),
            onError:  msg => System.Diagnostics.Debug.WriteLine($"[Framer] {msg}")
        );

        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
        {
            Handshake        = Handshake.None,
            ReadTimeout      = SerialPort.InfiniteTimeout,
            WriteTimeout     = 1000,
            ReceivedBytesThreshold = 1
        };

        _port.DataReceived += OnDataReceived;
        _port.Open();
    }

    public void Close()
    {
        if (_port is null) return;
        _port.DataReceived -= OnDataReceived;
        if (_port.IsOpen) _port.Close();
        _port.Dispose();
        _port    = null;
        _framer  = null;
    }

    public void SendRaw(byte[] bytes)
    {
        if (_port is null || !_port.IsOpen)
            throw new InvalidOperationException("序列埠尚未開啟。");
        _port.Write(bytes, 0, bytes.Length);
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (_port is null || !_port.IsOpen) return;
        int count = _port.BytesToRead;
        if (count <= 0) return;

        byte[] buf = new byte[count];
        _port.Read(buf, 0, count);

        RawReceived?.Invoke(this, buf);
        _framer?.Feed(buf);
    }

    public void Dispose() => Close();
}

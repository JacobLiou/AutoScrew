namespace UDL.Delta.IemdSd.Modbus;

internal interface ICommandMailbox
{
    Task SendCommandAsync(int commandCode, int[] requestWords, CancellationToken cancellationToken);
}

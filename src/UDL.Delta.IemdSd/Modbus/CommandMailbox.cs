using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Exceptions;

namespace UDL.Delta.IemdSd.Modbus;

internal sealed class CommandMailbox : ICommandMailbox
{
    private readonly IModbusTransport _transport;
    private readonly ILogger _logger;
    private readonly int _commandTimeoutMs;

    public CommandMailbox(IModbusTransport transport, IemdSdClientOptions options, ILogger logger)
    {
        _transport = transport;
        _logger = logger;
        _commandTimeoutMs = options.CommandTimeoutMs;
    }

    public async Task SendCommandAsync(int commandCode, int[] requestWords, CancellationToken cancellationToken)
    {
        if (requestWords.Length != 10)
            throw new ArgumentException("Command mailbox requires exactly 10 words.", nameof(requestWords));

        requestWords[0] = commandCode;
        requestWords[6] = 1;
        await _transport.WriteMultipleAsync(ModbusRegisterMap.CommandRequest, requestWords, cancellationToken)
            .ConfigureAwait(false);

        var deadline = DateTime.UtcNow.AddMilliseconds(_commandTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _transport.ReadHoldingAsync(ModbusRegisterMap.CommandResponse, 3, cancellationToken)
                .ConfigureAwait(false);
            if (response[0] == commandCode && response[1] == 1 && response[2] == 0)
                return;

            if (response[0] == commandCode && response[1] == 2)
            {
                throw new IemdSdCommunicationException($"Command #{commandCode} rejected by controller.")
                {
                    CommandCode = commandCode,
                    DeviceErrorCode = response[2],
                };
            }

            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
        }

        throw new IemdSdCommunicationException($"Command #{commandCode} timed out after {_commandTimeoutMs} ms.")
        {
            CommandCode = commandCode,
        };
    }

    public static int[] CreateEmptyRequest() => new int[10];

    public static int[] CreateRequest(
        int commandCode,
        int word1 = 0,
        int word2 = 0,
        int word3 = 0,
        int word4 = 0,
        int word5 = 0)
    {
        return new[] { commandCode, word1, word2, word3, word4, word5, 0, 0, 0, 0 };
    }

    public static void SetReportId(int[] request, uint reportId)
    {
        request[2] = (int)(reportId % 65536);
        request[3] = (int)(reportId / 65536);
    }
}

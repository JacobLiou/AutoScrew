namespace UDL.Delta.IemdSd.Modbus;

public sealed class ModbusCommandInvocation
{
    public int FunctionCode { get; init; }

    public int[] MailboxWords { get; init; } = CommandMailbox.CreateEmptyRequest();

    public int[]? WritePayload { get; init; }

    public uint? ReadWordCount { get; init; }

    public bool ReadbackFromCommandRequest { get; init; }

    public static ModbusCommandInvocation MailboxOnly(int functionCode, int word1 = 0, int word2 = 0, int word3 = 0, int word4 = 0, int word5 = 0) =>
        new()
        {
            FunctionCode = functionCode,
            MailboxWords = CommandMailbox.CreateRequest(functionCode, word1, word2, word3, word4, word5),
        };

    public static ModbusCommandInvocation WithWritePayload(int functionCode, int[] payload, int word1 = 0, int word2 = 0, int word3 = 0, int word4 = 0, int word5 = 0) =>
        new()
        {
            FunctionCode = functionCode,
            MailboxWords = CommandMailbox.CreateRequest(functionCode, word1, word2, word3, word4, word5),
            WritePayload = payload,
        };

    public static ModbusCommandInvocation WithReadPayload(int functionCode, uint readWordCount, int word1 = 0, int word2 = 0, int word3 = 0, int word4 = 0, int word5 = 0) =>
        new()
        {
            FunctionCode = functionCode,
            MailboxWords = CommandMailbox.CreateRequest(functionCode, word1, word2, word3, word4, word5),
            ReadWordCount = readWordCount,
        };

    public static ModbusCommandInvocation WithReportId(int functionCode, uint reportId, uint? readWordCount = null, int word4 = 0) =>
        new()
        {
            FunctionCode = functionCode,
            MailboxWords = CreateReportRequest(functionCode, reportId, word4),
            ReadWordCount = readWordCount,
        };

    private static int[] CreateReportRequest(int functionCode, uint reportId, int word4)
    {
        var req = CommandMailbox.CreateRequest(functionCode, word4: word4);
        CommandMailbox.SetReportId(req, reportId);
        return req;
    }
}

public sealed class ModbusCommandResult
{
    public int FunctionCode { get; init; }

    public int[] MailboxWords { get; init; } = [];

    public int[]? ReadPayload { get; init; }

    public int? ReadbackValue { get; init; }
}

namespace UDL.Delta.IemdSd.Modbus;

public enum ModbusCommandDirection
{
    Write,
    Read,
}

public enum ModbusCommandPayloadKind
{
    MailboxOnly,
    WriteThenMailbox,
    MailboxThenRead,
    MailboxThenReadCommandRequest,
}

public sealed class ModbusCommandDescriptor
{
    public ModbusCommandDescriptor(
        int code,
        string name,
        ModbusCommandDirection direction,
        ModbusCommandPayloadKind payloadKind,
        uint? typicalDataWordCount)
    {
        Code = code;
        Name = name;
        Direction = direction;
        PayloadKind = payloadKind;
        TypicalDataWordCount = typicalDataWordCount;
    }

    public int Code { get; }

    public string Name { get; }

    public ModbusCommandDirection Direction { get; }

    public ModbusCommandPayloadKind PayloadKind { get; }

    public uint? TypicalDataWordCount { get; }
}

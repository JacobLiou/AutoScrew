using System.Security.Cryptography;
using System.Text;

namespace UDL.Delta.IemdSd.Modbus;

/// <summary>Formats Modbus register payloads for communication diagnostics (full dump, no truncation).</summary>
internal static class ModbusPayloadFormatter
{
    private const int WordsPerGroup = 16;

    public static string Format(int startAddress, int[]? values)
    {
        if (values is null)
            return "-";

        if (values.Length == 0)
            return "Words=0 End=- Sha256=- Data=[]";

        var endAddress = startAddress + values.Length - 1;
        var sha = ComputeSha256Hex(values);
        var data = FormatData(values);
        return $"Words={values.Length} End=0x{endAddress:X4} Sha256={sha} Data=[{data}]";
    }

    public static string ComputeSha256Hex(int[] values)
    {
        var bytes = new byte[values.Length * 2];
        for (var i = 0; i < values.Length; i++)
        {
            var word = unchecked((ushort)values[i]);
            bytes[i * 2] = (byte)(word & 0xFF);
            bytes[i * 2 + 1] = (byte)(word >> 8);
        }

        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static string FormatData(int[] values)
    {
        var sb = new StringBuilder(values.Length * 6 + (values.Length / WordsPerGroup + 1) * 8);
        for (var i = 0; i < values.Length; i++)
        {
            if (i % WordsPerGroup == 0)
            {
                if (i > 0)
                    sb.Append(" | ");
                sb.Append('@');
                sb.Append(i.ToString("X4"));
                sb.Append(':');
            }
            else
            {
                sb.Append(' ');
            }

            sb.Append(unchecked((ushort)values[i]).ToString("X4"));
        }

        return sb.ToString();
    }
}

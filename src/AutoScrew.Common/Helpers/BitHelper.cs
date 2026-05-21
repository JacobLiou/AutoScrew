using System.Globalization;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class BitHelper
    {
        public static byte ToogleBit(byte InByte, byte BitNo)
        {
            int buff = InByte;
            buff ^= 1 << BitNo;
            return (byte)buff;
        }

        public static bool IsBitSet(byte InByte, byte BitNo)
        {
            return (InByte & 1 << BitNo) != 0;
        }

        /// <summary>
        /// Convert long to hex value
        /// </summary>
        public static string LongToHex(long val, OffSetPanelFixedWidth offsetwight = OffSetPanelFixedWidth.Dynamic) =>
            val.ToString(offsetwight == OffSetPanelFixedWidth.Dynamic
                ? ConstantReadOnly.HexStringFormat
                : ConstantReadOnly.HexLineInfoStringFormat, CultureInfo.InvariantCulture);

        public static string LongToString(long val, int saveBits = -1)
        {
            if (saveBits == -1) return $"{val}";

            //Char[] with fixed size is always
            var chs = new char[saveBits];
            for (var i = 1; i <= saveBits; i++)
            {
                chs[saveBits - i] = (char)(val % 10 + 48);
                val /= 10;
            }
            return new string(chs);
        }

        /// <summary>
        /// Convert Byte to Char (can be used as visible text)
        /// </summary>
        /// <remarks>
        /// Code from : https://github.com/pleonex/tinke/blob/master/Be.Windows.Forms.HexBox/ByteCharConverters.cs
        /// </remarks>
        public static char ByteToChar(byte val) => val > 0x1F && !(val > 0x7E && val < 0xA0) ? (char)val : '.';

        /// <summary>
        /// Convert Char to Byte
        /// </summary>
        public static byte CharToByte(char val) => (byte)val;

        /// <summary>
        /// Converts a byte array to a hex string. For example: {10,11} = "0A 0B"
        /// </summary>
        public static string ByteToHex(byte[] data)
        {
            if (data == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (var b in data)
            {
                var hex = ByteToHex(b);
                sb.Append(hex);
                sb.Append(' ');
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// Convert a byte to char[2].
        /// </summary>
        public static char[] ByteToHexCharArray(byte val)
        {
            var hexbyteArray = new char[2];
            ByteToHexCharArray(val, hexbyteArray);
            return hexbyteArray;
        }

        /// <summary>
        /// Fill the <paramref name="charArr"/> with hex char;
        /// </summary>
        /// <param name="charArr">The length of this value should be 2.</param>
        public static void ByteToHexCharArray(byte val, char[] charArr)
        {
            if (charArr == null)
                throw new ArgumentNullException(nameof(charArr));

            if (charArr.Length != 2)
                throw new ArgumentException($"The length of {charArr} should be 2.");

            charArr[0] = ByteToHexChar(val >> 4);
            charArr[1] = ByteToHexChar(val - ((val >> 4) << 4));
        }

        /// <summary>
        /// Convert a byte to Hex char,i.e,10 = 'A'
        /// </summary>
        public static char ByteToHexChar(int val) =>
            val < 10
                ? (char)(48 + val)
                : (val switch
                {
                    10 => 'A',
                    11 => 'B',
                    12 => 'C',
                    13 => 'D',
                    14 => 'E',
                    15 => 'F',
                    _ => 's',
                });

        /// <summary>
        /// Converts the byte to a hex string. For example: "10" = "0A";
        /// </summary>
        public static string ByteToHex(byte val) => new(ByteToHexCharArray(val));

        /// <summary>
        /// Convert byte to ASCII string
        /// </summary>
        public static string BytesToString(byte[] buffer, ByteToString converter = ByteToString.ByteToCharProcess)
        {
            if (buffer == null) return string.Empty;

            switch (converter)
            {
                case ByteToString.AsciiEncoding:
                    return Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                case ByteToString.ByteToCharProcess:
                    var builder = new StringBuilder();

                    foreach (var @byte in buffer)
                        builder.Append(ByteToChar(@byte));

                    return builder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts the hex string to an byte array. The hex string must be separated by a space char ' '. If there is any invalid hex information in the string the result will be null.
        /// </summary>
        public static byte[] HexToByte(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return null;

            hex = hex.Trim();
            var hexArray = hex.Split(' ');
            var byteArray = new byte[hexArray.Length];

            for (var i = 0; i < hexArray.Length; i++)
            {
                var hexValue = hexArray[i];
                var (isByte, val) = HexToUniqueByte(hexValue);

                if (!isByte) return null;

                byteArray[i] = val;
            }

            return byteArray;
        }

        /// <summary>
        /// Return Tuple (bool, byte) that bool represent if is a byte
        /// </summary>
        public static (bool success, byte val) HexToUniqueByte(string hex) =>
            (byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var val), val);

        /// <summary>
        /// Convert a hex string to long.
        /// </summary>
        /// <return>
        /// Return (true, [position])
        /// Return (false, -1) on error
        /// </return>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0054:Utiliser une attribution composée", Justification = "<En attente>")]
        public static (bool success, long position) HexLiteralToLong(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return (false, -1);

            var i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X')
                ? 2
                : 0;

            long value = 0;

            while (i < hex.Length)
            {
                #region convert

                int x = hex[i++];

                if
                    (x >= '0' && x <= '9') x = x - '0';
                else if
                    (x >= 'A' && x <= 'F') x = x - 'A' + 10;
                else if
                    (x >= 'a' && x <= 'f') x = x - 'a' + 10;
                else
                    return (false, -1);

                value = 16 * value + x;

                #endregion convert
            }

            return (true, value);
        }

        /// <summary>
        /// Check if is an hexa string
        /// </summary>
        public static (bool success, long value) IsHexValue(string hexastring) => HexLiteralToLong(hexastring);

        /// <summary>
        /// Check if is an hexa byte string
        /// </summary>
        public static (bool success, byte[] value) IsHexaByteStringValue(string hexastring) =>
            HexToByte(hexastring) == null
                ? (false, null)
                : (true, byteArray: HexToByte(hexastring));

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        public static byte[] StringToByte(string str) => str.Select(CharToByte).ToArray();

        /// <summary>
        /// Convert String to hex string For example: "barn" = "62 61 72 6e"
        /// </summary>
        public static string StringToHex(string str) => ByteToHex(StringToByte(str));

        public static class ConstantReadOnly
        {
            public static readonly string HexLineInfoStringFormat = "x8";
            public static readonly string Hex2StringFormat = "x2";
            public static readonly string HexStringFormat = "x";
            public static readonly string DefaultHex8String = "0x00000000";
            public static readonly string DefaultHex2String = "0x00";

            public const long Largefilelength = 52_428_800L; //50 MB
            public const int Copyblocksize = 131_072; //128 KB
            public const int Findblocksize = 1_048_576; //1 MB
        }
    }

    /// <summary>
    /// ByteAction used for ByteModified class
    /// </summary>
    public enum ByteAction
    {
        Nothing,
        Added,
        Deleted,
        Modified,

        /// <summary>
        /// Used in ByteProvider for get list
        /// </summary>
        All
    }

    /// <summary>
    /// Used for coloring mode of selection
    /// </summary>
    public enum FirstColor
    {
        HexByteData,
        StringByteData
    }

    /// <summary>
    /// Mode of Copy/Paste
    /// </summary>
    public enum CopyPasteMode
    {
        Byte,
        HexaString,
        AsciiString,
        TblString,
        CSharpCode,
        VbNetCode,
        JavaCode,
        CCode,
        FSharpCode,
        PascalCode
    }

    /// <summary>
    /// Used with Copy to code fonction for language are similar to C.
    /// </summary>
    internal enum CodeLanguage
    {
        C,
        CSharp,
        Java,
        FSharp,
        Vbnet,
        Pascal
    }

    /// <summary>
    /// Used for check label are selected et next label to select...
    /// </summary>
    public enum KeyDownLabel
    {
        FirstChar,
        SecondChar,
        ThirdChar,
        FourthChar,
        FifthChar,
        SixthChar,
        SeventhChar,
        EighthChar,
        Ninth,
        Tenth,
        Eleventh,
        Twelfth,
        Thirteenth,
        Fourteenth,
        Fifteenth,
        Sixteenth,
        Seventeenth,
        Eighteenth,
        Ninteenth,
        Twentieth,
        TwentyFirst,
        TwentySecond,
        TwentyThird,
        TwentyFourth,
        TwentyFifth,
        TwentySixth,
        TwentySeventh,
        TwentyEighth,
        TwentyNinth,
        Thirtieth,
        ThirtyFirst,
        ThirtySecond,
        NextPosition
    }

    public enum ByteToString
    {
        /// <summary>
        /// Build-in convertion mode. (recommended)
        /// </summary>
        ByteToCharProcess,

        /// <summary>
        /// System.Text.Encoding.ASCII string encoder
        /// </summary>
        AsciiEncoding
    }

    /// <summary>
    /// Scrollbar marker
    /// </summary>
    public enum ScrollMarker
    {
        Nothing,
        SearchHighLight,
        Bookmark,
        SelectionStart,
        ByteModified,
        ByteDeleted,
        TblBookmark
    }

    /// <summary>
    /// Type are opened in byteprovider
    /// </summary>
    public enum ByteProviderStreamType
    {
        File,
        MemoryStream,
        Nothing
    }

    /// <summary>
    /// Type of character are used
    /// </summary>
    public enum CharacterTableType
    {
        Ascii,
        TblFile
    }

    /// <summary>
    /// Used for control the speed of mouse wheel
    /// </summary>
    public enum MouseWheelSpeed
    {
        VerySlow = 1,
        Slow = 3,
        Normal = 5,
        Fast = 7,
        VeryFast = 9,
        System
    }

    /// <summary>
    /// IByteControl spacer width
    /// </summary>
    public enum ByteSpacerWidth
    {
        VerySmall = 1,
        Small = 3,
        Normal = 6,
        Large = 9,
        VeryLarge = 12
    }

    [Flags]
    public enum ByteSpacerGroup
    {
        TwoByte = 2,
        FourByte = 4,
        SixByte = 6,
        EightByte = 8
    }

    public enum ByteSpacerPosition
    {
        HexBytePanel,
        StringBytePanel,
        Both,
        Nothing
    }

    public enum ByteSpacerVisual
    {
        Empty,
        Line,
        Dash
    }

    /// <summary>
    /// Used with the view mode of HexByte, header or position.
    /// </summary>
    public enum DataVisualType
    {
        Hexadecimal,    //Editable
        Decimal,        //Editable
        Binary        //Editable
    }

    /// <summary>
    /// Used with the view mode of HexByte, header or position.
    /// </summary>
    public enum DataVisualState
    {
        Default,
        Origin,
        Changes,
        ChangesPercent
    }

    public enum ByteSizeType
    {
        Bit8,       // editable
        Bit16,      // editable
        Bit32       // editable
    }

    public enum ByteOrderType
    {
        LoHi,
        HiLo //not editable
    }

    /// <summary>
    /// Used to select the visual of the offset panel
    /// </summary>
    public enum OffSetPanelType
    {
        OffsetOnly,
        LineOnly,
        Both
    }

    /// <summary>
    /// Used to fix the wigth of the offset panel
    /// </summary>
    public enum OffSetPanelFixedWidth
    {
        Dynamic,
        Fixed
    }

    /// <summary>
    /// Used to set the the caret mode
    /// </summary>
    public enum CaretMode
    {
        Insert,
        Overwrite
    }

    /// <summary>
    /// Used to set how many line will be preloaded at control creation
    /// </summary>
    public enum PreloadByteInEditor
    {
        /// <summary>
        /// Load nothing at control start
        /// </summary>
        None,

        /// <summary>
        /// Load maximum of visible line in control view
        /// </summary>
        MaxVisibleLine,

        /// <summary>
        /// Add 10 lines to MaxVisible line
        /// </summary>
        MaxVisibleLineExtended,

        /// <summary>
        /// Load the maximum of line to fit to the screen
        /// </summary>
        MaxScreenVisibleLine,

        /// <summary>
        /// Load MaxScreenVisibleLine at control creation and the others lines will be loaded at first load of file/stream
        /// </summary>
        MaxScreenVisibleLineAtDataLoad
    }
}
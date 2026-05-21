namespace ProtocolSimulationTest.Common.Helpers
{
    public static class ConvertUtil
    {
        public static byte[] SmallBigConvert(byte[] input, int skip, int take)
        {
            byte[] array = input.Skip(skip).Take(take).ToArray();
            Array.Reverse(array);
            return array;
        }

        public static ushort GetUInt16FromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToUInt16(SmallBigConvert(data, p, 2), 0);
        }

        public static uint GetUIntFromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToUInt32(SmallBigConvert(data, p, 4), 0);
        }

        public static ulong GetUInt64FromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToUInt64(SmallBigConvert(data, p, 8), 0);
        }

        public static short GetInt16FromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToInt16(SmallBigConvert(data, p, 2), 0);
        }

        public static int GetIntFromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToInt32(SmallBigConvert(data, p, 4), 0);
        }

        public static long GetInt64FromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToInt64(SmallBigConvert(data, p, 8), 0);
        }

        public static float GetFloatFromBigEndianBytes(byte[] data, int p)
        {
            return BitConverter.ToSingle(SmallBigConvert(data, p, 4), 0);
        }

        public static double GetDouble(byte[] data, int p)
        {
            return BitConverter.ToDouble(SmallBigConvert(data, p, 8), 0);
        }
    }
}
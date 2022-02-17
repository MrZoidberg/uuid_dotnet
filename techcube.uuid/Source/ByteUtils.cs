using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("techcube.uuid.Test")]

namespace techcube.uuid
{
    internal static class ByteUtils
    {
        public static BitArray BitStringToBitArray(string bitString, bool MSB = false)
        {
            BitArray bitArray = new(bitString.Length);

            for (int i = 0; i < bitString.Length; i++)
            {
                bitArray.Set(i, bitString[MSB ? i : bitString.Length - 1 - i] == '1');
            }

            return bitArray;
        }

        public static byte[] BitStringToByteArray(string bitString, bool MSB = false)
        {
            //return BitStringToBitArray(bitString, MSB).ToBytesSpan();
            byte[] bytes = new byte[bitString.Length / 8];
            for (int i = 0; i < bitString.Length; i++)
            {
                int byteIndex = i / 8;
                int bitInByteIndex = i % 8;
                byte mask = (byte)(1 << bitInByteIndex);
                bool value = bitString[MSB ? i : bitString.Length - 1 - i] == '1';
                if (value)
                {
                    bytes[byteIndex] |= mask;
                }
            }

            return bytes;


        }

        private static readonly uint[] Lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }

            return result;
        }

        public static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            var lookup32 = Lookup32;
            var result = ArrayPool<char>.Shared.Rent(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }

            var ret = new string(result);
            ArrayPool<char>.Shared.Return(result);
            return ret;
        }
    }
}
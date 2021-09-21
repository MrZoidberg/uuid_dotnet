using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace techcube.uuid
{
    internal static class BitArrayExtensions
    {
        public static string ToBitString(this BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = bits.Count - 1; i >= 0; i--)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }
        
        public static int ReadInt32LittleEndian(this BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.", nameof(bitArray));

            byte[] array = new byte[4];
            bitArray.CopyTo(array, 0);
            return BinaryPrimitives.ReadInt32LittleEndian(array);
        }
        
        public static UInt32 ReadUInt32LittleEndian(this BitArray bitArray)
        {
            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.", nameof(bitArray));

            byte[] array = new byte[4];
            bitArray.CopyTo(array, 0);
            return BinaryPrimitives.ReadUInt32LittleEndian(array);
        }
        
        public static byte ReadByte(this BitArray bitArray)
        {
            if (bitArray.Length > 8)
                throw new ArgumentException("Argument length shall be at most 8 bits.", nameof(bitArray));

            byte[] array = new byte[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static BitArray CopySlice(this BitArray source, int offset, int length)
        {
            // Urgh: no CopyTo which only copies part of the BitArray
            BitArray destination = new(length);
            for (int i = 0; i < length; i++)
            {
                destination[i] = source[offset + i];
            }

            return destination;
        }

        public static BitArray CopySlice(this BitArray source, BitArray destination, int offset, int length, int destinationOffset)
        {
            for (int i = 0; i < length; i++)
            {
                destination[i + destinationOffset] = source[offset + i];
            }

            return destination;
        }
        
        public static IEnumerable<byte> ToBytes(this BitArray bits, bool msb = false)
        {
            int bitCount = 7;
            int outByte = 0;

            foreach (bool bitValue in bits)
            {
                if (bitValue)
                    outByte |= msb ? 1 << bitCount : 1 << (7 - bitCount);
                if (bitCount == 0)
                {
                    yield return (byte) outByte;
                    bitCount = 8;
                    outByte = 0;
                }
                bitCount--;
            }
            // Last partially decoded byte
            if (bitCount < 7)
                yield return (byte) outByte;
        }
        
        public static Span<byte> ToBytesSpan(this BitArray bits, bool msb = false)
        {
            int bitCount = 7;
            int outByte = 0;

            byte[] bytes = new byte[bits.Length / 8];
            var i = 0;
            foreach (bool bitValue in bits)
            {
                if (bitValue)
                    outByte |= msb ? 1 << bitCount : 1 << (7 - bitCount);
                if (bitCount == 0)
                {
                    bytes[i] = (byte)outByte;
                    i++;
                    bitCount = 8;
                    outByte = 0;
                }
                bitCount--;
            }
            // Last partially decoded byte
            if (bitCount < 7)
            {
                bytes[i] = (byte)outByte;
            }

            return bytes;
    }
    }
}
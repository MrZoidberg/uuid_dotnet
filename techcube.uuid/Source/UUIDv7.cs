using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace techcube.uuid
{
        
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct UUIDv7 : IComparable, IComparable<UUIDv7>, IEquatable<UUIDv7>
    {
        private const int SecBits = 36; // unixts at second precision
        private const int SubsecBits = 30; // enough to represent NS
        private const int VersionBits = 4; // '0111' for ver 7
        private const int VariantBits = 2; // '10' Static for UUID
        private const int SequenceBits = 8; // Enough for 256 UUIDs per NS
        private const int NodeBits = 128 - SecBits - SubsecBits - VersionBits - VariantBits - SequenceBits; // 48
        private const decimal SubsecDecimalDivisor = 1_000_000_000;

        private const string UuidVersion = "0111"; // ver 7
        private static byte uuidVersionBytes = 7;
        private const string UuidVariant = "10";
        private static byte uuidVariantBytes = 2;

        private static object lockObject = new();
        private static long lastV7Timestamp;
        private static long lastSequence;
        private static long sequenceCounter;

        //public static readonly UUIDv7 Empty = EmptyUuid();

        private readonly UInt32 secPart;
        private readonly int subSecPart;
        private readonly long counterPart;
        private readonly byte versionPart;
        private readonly byte variantPart;
        private readonly byte[] randomStringPart;
        private readonly byte[] uuidBytes;

        public DateTimeOffset Timestamp
        {
            get
            {
                int ms = (int)(subSecPart / SubsecDecimalDivisor);
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(secPart).AddMilliseconds(ms);
            }
        }

        public long TimestampMs
        {
            get
            {
                long ms = (long)(subSecPart / SubsecDecimalDivisor);
                return (long)secPart * 1000 + ms;
            }
        }

        public long TimestampNs => secPart * (long)SubsecDecimalDivisor + subSecPart;

        public byte Version => versionPart;

        public byte Variant => variantPart;
        public UUIDv7(ReadOnlySpan<byte> bytes)
        {
            if ((uint)bytes.Length != 16)
            {
                throw new ArgumentException("Input array should be 16 bytes in length.", nameof(bytes));
            }

            uuidBytes = bytes.ToArray();
            var bits = new BitArray(uuidBytes);
            int len = bits.Count;
            secPart = bits.CopySlice(len - 32, 32).ReadUInt32LittleEndian();
            var subSecBits = new BitArray(SubsecBits);
            bits.CopySlice(subSecBits, len - SecBits - 12, 12, 6 + 12);
            bits.CopySlice(subSecBits, len - SecBits - 12 - 4 - 12, 12, 6);
            bits.CopySlice(subSecBits, len - SecBits - 12 - 4 - 12 - VariantBits - 6, 6, 0);

            subSecPart = subSecBits.ReadInt32LittleEndian();
            counterPart = bits.CopySlice(len - SecBits - 12 - 4 - 12 - VariantBits - 6 - SequenceBits, SequenceBits).ReadByte();
            versionPart = bits.CopySlice(len - SecBits - 12 - VersionBits, VersionBits).ReadByte();
            variantPart = bits.CopySlice(len - SecBits - 12 - 4 - 12 - VariantBits, VariantBits).ReadByte();
            randomStringPart = bits.CopySlice(0, NodeBits).ToBytesSpan().ToArray();
        }

        private UUIDv7(UInt32 sec, int subSec, long counter, byte[] randomString, byte version, byte variant, byte[] uuid)
        {
            secPart = sec;
            subSecPart = subSec;
            counterPart = counter;
            randomStringPart = randomString;
            uuidBytes = uuid;
            versionPart = version;
            variantPart = variant;
        }

        public int CompareTo(UUIDv7 other)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public override bool Equals(object? other)
        {
            UUIDv7 g;
            // Check that o is a Guid first
            if (other == null || !(other is UUIDv7))
                return false;
            g = (UUIDv7)other;

            // Now compare each of the elements
            return g.secPart == secPart && g.subSecPart == subSecPart && g.counterPart == counterPart && g.randomStringPart == randomStringPart;
        }

        public override int GetHashCode()
        {
            return uuidBytes.GetHashCode();
        }

        public bool Equals(UUIDv7 other)
        {
            return other.uuidBytes.SequenceEqual(uuidBytes);
        }

        public override string ToString()
        {
            string hexString = ByteUtils.ByteArrayToHexViaLookup32(uuidBytes);
            return string.Join('-', hexString[..8], hexString[8..12], hexString[12..16], hexString[16..20], hexString[20..32]);
        }

        public ReadOnlySpan<byte> ToByteArray()
        {
            return (byte[])this;
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public static explicit operator Guid(UUIDv7 uuid)
        {
            return new Guid(uuid.uuidBytes);
        }
        
        public static explicit operator byte[](UUIDv7 uuid)
        {
            return (byte[])uuid.uuidBytes.Clone();
        }

        public static UUIDv7 NewUuid()
        {
            return NewUuid(TimeUtils.GetNanoseconds());
        }

        public static UUIDv7 NewUuid(long timestamp)
        {
            // Timestamp Work
            // Produces unix epoch with nanosecond precision

            var subsec_decimal_digits = (decimal)9; //Last 9 digits of are subsection precision
            var integerPart = (UInt32)(timestamp / SubsecDecimalDivisor); // Get seconds
            var sec = integerPart;

            // Conversion to decimal
            var fractionalPart = Math.Round((timestamp % SubsecDecimalDivisor) / SubsecDecimalDivisor, (int)subsec_decimal_digits);
            var subsec = (int)Math.Round(fractionalPart * (decimal)Math.Pow(2, SubsecBits)); // Convert to 30 bit int, round
            
            var testTimestamp = sec + fractionalPart.ToString("N9")[^9..^0];
            Debug.Assert(testTimestamp == timestamp.ToString());

            // Binary Conversions
            // Need subsec_a (12 bits), subsec_b (12-bits), and subsec_c (leftover bits starting subsec_seq_node)
            var unixts = Convert.ToString(sec, 2).PadLeft(32, '0');
            unixts += "0000";
            var subsecBinary = Convert.ToString(subsec, 2).PadLeft(30, '0');
            var subsecA = subsecBinary[..12];
            var subsecBC = subsecBinary[^18..];
            var subsecB = subsecBC[..12];
            var subsecC = subsecBinary[^6..];

            // Sequence Work
            // Sequence starts at 0, increments if timestamp is the same, the sequence increments by 1
            // Resets if timestamp int is larger than _last_v7timestamp used for UUID generation
            // Will be 8 bits for NS timestamp
            long counter;
            Monitor.Enter(lockObject);
            try
            {
                if (timestamp <= lastV7Timestamp)
                {
                    sequenceCounter += 1;
                }

                if (timestamp > lastV7Timestamp)
                {
                    sequenceCounter = 0;
                }

                counter = sequenceCounter;
                lastV7Timestamp = timestamp;
                lastSequence = sequenceCounter;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            var sequenceCounterBin = Convert.ToString(counter, 2).PadLeft(8, '0')[^8..];

            // Random Node Work

            byte[] rndArray = new byte[NodeBits / 8];
            RandomNumberGenerator.Create().GetBytes(rndArray);
            var randomBinary = new BitArray(rndArray);
            var randomBinaryStr = randomBinary.ToBitString();

            var subsecSeqNode = subsecC + sequenceCounterBin + randomBinaryStr;

            // Formatting Work
            // Bin merge and Int creation
            var uuidBin = unixts + subsecA + UuidVersion + subsecB + UuidVariant + subsecSeqNode;
            var uuidBytes = ByteUtils.BitStringToByteArray(uuidBin);

            return new UUIDv7(sec, subsec, counter, rndArray, uuidVersionBytes, uuidVariantBytes, uuidBytes);
        }

        private static UUIDv7 EmptyUuid()
        {
            var b = new byte[16];
            return new UUIDv7(b);
        }
    }
}
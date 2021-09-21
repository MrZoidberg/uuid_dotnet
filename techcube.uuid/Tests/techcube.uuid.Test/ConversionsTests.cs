using System;
using System.Buffers.Binary;
using System.Collections;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace techcube.uuid.Test
{
    public class ConversionsTests
    {
        private readonly ITestOutputHelper output;

        public ConversionsTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Theory]
        [InlineData("01100001010010001111100000101110")]
        public void BinaryConversionTest(string binaryString)
        {
            var uuidBytes = ByteUtils.BitStringToByteArray(binaryString);
            var bits = new BitArray(uuidBytes);
            var bitString = bits.ToBitString();

            binaryString.Should().Be(bitString);
        }

        [Theory]
        [InlineData(1632173846, "01100001010010001111111100010110")]
        [InlineData(4294967295, "11111111111111111111111111111111")]
        public void UInt32ConversionTest(uint value, string binaryString)
        {
            var strBin = Convert.ToString(value, 2).PadLeft(32, '0');
            strBin.Should().Be(binaryString);
            
            var bits = ByteUtils.BitStringToBitArray(strBin);
            bits.ToBitString().Should().Be(binaryString);

            BinaryPrimitives.ReadUInt32LittleEndian(bits.ToBytesSpan()).Should().Be(value);
            bits.ReadUInt32LittleEndian().Should().Be(value);
        }
        
        [Theory]
        [InlineData(1632173846, "01100001010010001111111100010110")]
        [InlineData(2147483647, "01111111111111111111111111111111")]
        public void Int32ConversionTest(int value, string binaryString)
        {
            var strBin = Convert.ToString(value, 2).PadLeft(32, '0');
            strBin.Should().Be(binaryString);
            
            var bits = ByteUtils.BitStringToBitArray(strBin);
            bits.ToBitString().Should().Be(binaryString);

            BinaryPrimitives.ReadInt32LittleEndian(bits.ToBytesSpan()).Should().Be(value);
            bits.ReadInt32LittleEndian().Should().Be(value);
        }
    }
}
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace techcube.uuid.Test
{
    public class UUIDv7Tests
    {
        private readonly ITestOutputHelper output;

        public UUIDv7Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            List<UUIDv7> masterList = new List<UUIDv7>();
            Dictionary<int, UUIDv7> masterDict = new Dictionary<int, UUIDv7>();
            for (int i = 0; i < 1000; i++)
            {
                var uuid = UUIDv7.NewUuid();
                string uuidString = uuid.ToString();
                uuid.Version.Should().Be(7);

                masterDict[i] = uuid;

                var bytes = uuid.ToByteArray();
                var uuid2 = new UUIDv7(bytes);
                uuid.Timestamp.Should().Be(uuid2.Timestamp);
                uuid.TimestampMs.Should().Be(uuid2.TimestampMs);
                uuid.TimestampNs.Should().Be(uuid2.TimestampNs);
                uuid.Version.Should().Be(uuid2.Version);
                uuid.Variant.Should().Be(uuid2.Variant);
                uuid.RandomBytes.Should().Equal(uuid2.RandomBytes);
            }
        }
    }
}
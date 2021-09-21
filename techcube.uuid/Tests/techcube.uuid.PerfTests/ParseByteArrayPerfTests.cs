using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace techcube.uuid.PerfTests
{
    [SimpleJob(RuntimeMoniker.Net50)]
    public class ParseByteArrayPerfTests
    {
        private byte[] bytes; 

        [IterationSetup]
        public void Setup()
        {
            bytes = (byte[])UUIDv7.NewUuid();
        }
        
        [Benchmark(Description = "UUIDv7 parse byte array test")]
        public UUIDv7 UUIdv7Test()
        {
            return new UUIDv7(bytes);
        }
        
        [Benchmark(Baseline = true, Description = "GUID parse byte array test")]
        public Guid GuidTest()
        {
            return new Guid(bytes);
        }
    }
}
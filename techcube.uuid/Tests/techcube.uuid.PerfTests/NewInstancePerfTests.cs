using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace techcube.uuid.PerfTests
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net50)]
    public class NewInstancePerfTests
    {
        public NewInstancePerfTests()
        {
            var tmp = UUIDv7.Empty.ToString();
            tmp = Guid.Empty.ToString();
        }
        
        [Benchmark(Description = "UUIDv7 new instance generation test")]
        public string UUIdv7Test()
        {
            UUIDv7 uuiDv7 = UUIDv7.NewUuid();
            return uuiDv7.ToString();
        }
        
        [Benchmark(Baseline = true, Description = "GUID new instance generation test")]
        public string GuidTest()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}
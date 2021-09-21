using System;
using BenchmarkDotNet.Running;

namespace techcube.uuid.PerfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<NewInstancePerfTests>();
            BenchmarkRunner.Run<ParseByteArrayPerfTests>();
            
        }
    }
}
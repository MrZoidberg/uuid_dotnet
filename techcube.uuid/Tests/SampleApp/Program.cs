using System;
using techcube.uuid;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            for (int i = 0; i < 100000; i++)
            {
                UUIDv7 uuiDv7 = UUIDv7.NewUuid();
                Console.WriteLine(uuiDv7.ToString());
            }
        }
    }
}
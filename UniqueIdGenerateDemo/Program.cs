using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniqueIdGenerate;

namespace UniqueIdGenerateDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var IdGeneration = new DistributedSystemUniqueIdGeneration(1);
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 3000; i++)
            {
                var id = IdGeneration.NextId();
                Console.WriteLine("{0} : {1}",id, Convert.ToString(id,2));
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}

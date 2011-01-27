using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            int count = 1000;
            Console.WriteLine(string.Format("Transforming {0} templates", count));

            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                TextTemplateTransformer transformer = new TextTemplateTransformer();
                transformer.TransformFile(@"Template1.stt");
                Console.Write(".");
            }

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine(string.Format("Completed in {0}", stopwatch.Elapsed));
            Console.WriteLine(string.Format("Averaging {0} each", TimeSpan.FromMilliseconds(((double)stopwatch.Elapsed.TotalMilliseconds / (double)count))));
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }
    }
}

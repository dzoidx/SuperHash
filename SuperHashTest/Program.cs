using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SuperHash
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("File not found");
                Console.ReadKey();
                return;
            }

            long fileSize;
            using (var f = File.OpenRead(args[0]))
            {
                fileSize = f.Length;
                Console.WriteLine("File size: " + fileSize);
            }

            var sw = new Stopwatch();
            sw.Restart();
            var sh = new SuperHash<SHA1Managed>(args[0]);
            sh.Finished += chunks =>
            {
                Console.WriteLine("time: {0} ms", sw.ElapsedMilliseconds);
                Console.WriteLine("speed: {0:F} Gb/s", (double)fileSize / sw.ElapsedMilliseconds / 1000.0 / 1000.0);
                using (var f = new StreamWriter(File.OpenWrite(args[0] + ".superhash")))
                {
                    var count = 0;
                    foreach (var chunk in chunks)
                    {
                        f.WriteLine(chunk.ToString(count++));
                    }
                }
            };
            sh.Start();

            var hash = SHA1.Create();
            sw.Restart();
            var h = hash.ComputeHash(File.OpenRead(args[0]));
            Console.WriteLine("[Not threaded hash]");
            Console.WriteLine("time: {0} ms", sw.ElapsedMilliseconds);
            Console.WriteLine("speed: {0:F} Gb/s", (double)fileSize / sw.ElapsedMilliseconds / 1000.0 / 1000.0);
            using (var f = new StreamWriter(File.OpenWrite(args[0] + ".sha1")))
            {
                f.WriteLine(BitConverter.ToString(h).Replace("-", ""));
            }

            Console.ReadKey();
        }
    }
}

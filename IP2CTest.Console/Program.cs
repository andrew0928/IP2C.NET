using IP2C.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP2CTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Client _client = new Client(new Uri(args[0]));

            //Verirfy the result by result from ip2c.org
            var lines = File.ReadAllLines("TestIPs.txt");

            foreach (var line in lines)
            {
                var p = line.Split(',');
                if (string.IsNullOrEmpty(p[1])) p[1] = "--";

                var result = _client.FindIPCountry(p[0]);

                System.Console.WriteLine($"Expect: {result.CountryCode}, Actual: {p[1]}, ServerInfo: {result.ServerInfo.ServerAddress}, {result.ServerInfo.Version}");
            }

        }
    }
}

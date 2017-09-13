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

                var code1 = _client.FindIPCountry(p[0]).CountryCode;

                Assert_AreEqual(code1, p[1]);
            }

        }

        static void Assert_AreEqual(string expected, string actual)
        {
            string result = (expected != actual) ? "FAIL" : "PASS";
            System.Console.WriteLine($"[{result}]: Excected ({expected}), but ({actual})!");
        }
    }
}

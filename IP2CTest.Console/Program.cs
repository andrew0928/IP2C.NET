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
            #region parsing argument(s)

            // usage: IP2CTest.Console.exe [-consul http://localhost:8500]

            string consulAddress = "http://localhost:8500";

            for (int index = 0; index < args.Length; index += 2)
            {
                switch (args[index])
                {
                    case "-consul":
                        consulAddress = args[index + 1];
                        break;

                    default:
                        break;
                }
            }
            #endregion
            ServiceClient.Init(consulAddress);
            Client _client = new Client(); //Client(new Uri(args[0]));

            //Verirfy the result by result from ip2c.org
            var lines = File.ReadAllLines("TestIPs.txt");

            while(true)
            foreach (var line in lines)
            {
                var p = line.Split(',');
                if (string.IsNullOrEmpty(p[1])) p[1] = "--";

                try
                {
                    var result = _client.FindIPCountry(p[0]);
                    System.Console.WriteLine($"Expect: {result.CountryCode}, Actual: {p[1]}, ServerInfo: {result.ServerInfo.ServerAddress}, {result.ServerInfo.Version}");
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine($"Exception: {ex}");
                }

                Task.Delay(100).Wait();
            }

        }
    }
}

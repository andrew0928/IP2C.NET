using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using IP2C.Net;
using System.Collections.Generic;
using System.Linq;
using IP2C.SDK;

namespace IP2CTest
{
    [TestClass]
    public class SDKTest
    {
        private Client _client = null;

        [TestInitialize]
        public void Init()
        {
            this._client = 
                new Client(new Uri(@"http://localhost:62676/"));
                //new Client(new Uri(@"http://172.24.225.100/"));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadPathTest()
        {
            var finder = new IPCountryFinder("C:\\NOT_THERE\\default-ipdb.csv");
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ParseTest()
        {
            var finder = new IPCountryFinder("IpToCountryBad.csv");
        }
        [TestMethod]
        public void FindTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var finder = new IPCountryFinder("default-ipdb.csv");
            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 1000);

            var ci = this._client.FindIPCountry("168.95.1.1");

            Assert.AreEqual(ci.CountryCode, "TW");
            Assert.AreEqual(ci.CountryName, "Taiwan; Republic of China (ROC)");
        }


        //[TestMethod]
        //public void SDKFindTest()
        //{
        //    var client = new Client(new Uri("http://localhost:62676/"));

        //    Assert.AreEqual(client.FindIPCountry("168.95.1.1").CountryCode, "TW");
        //    Assert.AreEqual(client.FindIPCountry("168.95.1.1").CountryCode, "TW");
        //    Assert.AreEqual(client.FindIPCountry("168.95.1.1").CountryCode, "TW");
        //    Assert.AreEqual(client.FindIPCountry("168.95.1.1").CountryCode, "TW");
        //    Assert.AreEqual(client.FindIPCountry("168.95.1.1").CountryCode, "TW");
        //}

        [TestMethod]
        public void ResultCheck()
        {
            //Verirfy the result by result from ip2c.org
            var finder = new IPCountryFinder("default-ipdb.csv");
            var sw = new Stopwatch();
            var lines = File.ReadAllLines("TestIPs.txt");
            sw.Start();
            foreach (var line in lines)
            {
                var p = line.Split(',');
                if (string.IsNullOrEmpty(p[1])) p[1] = "--";

                var code1 = this._client.FindIPCountry(p[0]).CountryCode;
                var code2 = finder.GetCountryCode(p[0]);

                Assert.AreEqual(code1, code2);
                Assert.AreEqual(code1, p[1]);
            }
            sw.Stop();
            Debug.WriteLine($@"Time={sw.ElapsedMilliseconds:n0}ms 
Count={lines.Count()}
Average ={(double)sw.ElapsedMilliseconds / lines.Count():n2}ms");
            
        }
        
//        public void GenTestIps()
//        {
//            //Get random IPs from https://www.randomlists.com/ip-addresses
//            var src = @"48.134.176.160
//246.91.15.191
//26.205.18.46";
//            WebClient wc = new WebClient();
//            using (StreamWriter sw = new StreamWriter("TestIp2.txt"))
//            {
//                foreach (var ip in src.Split('\n'))
//                {
//                    var res = wc.DownloadString("http://www.ip2c.org/" + ip);
//                    var p = res.Split(';');
//                    sw.WriteLine($"{ip},{p[1]}");
//                }
//            }
//        }

        

    }
}

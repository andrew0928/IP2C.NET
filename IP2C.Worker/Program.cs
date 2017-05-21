using IP2C.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IP2C.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length > 0 && args[0] == "--watch")
            if (true)
            {
                // watch mode
                DateTime start = new DateTime(2000, 1, 1, 15, 40, 0);
                TimeSpan period = //TimeSpan.FromHours(1.0);
                    TimeSpan.FromMinutes(3.0);

                while (true)
                {
                    TimeSpan wait = TimeSpan.FromMilliseconds(period.TotalMilliseconds - (DateTime.Now - start).TotalMilliseconds % period.TotalMilliseconds);
                    Console.WriteLine("wait: {0} (until: {1})", wait, DateTime.Now.Add(wait));
                    Task.Delay(wait).Wait();
                    UpdateFile(@"http://software77.net/geo-ip/?DL=1", @"d:\ip2c.csv");
                }
                
            }
            else
            {
                // update once
                UpdateFile(@"http://software77.net/geo-ip/?DL=1", @"d:\ip2c.csv");
            }
        }


        static void UpdateFile(string url, string file)
        {
            Console.WriteLine("-update file: {0}", DateTime.Now);
            // download url->temp
            // test temp
            // if (pass)
            //  - move temp -> file

            string temp = Path.ChangeExtension(file, ".temp");
            string back = Path.ChangeExtension(file, ".bak");

            if (File.Exists(temp)) File.Delete(temp);

            if (DownloadAndExtractGZip(url, temp))
            {
                if (TestFile(temp))
                {
                    if (File.Exists(back)) File.Delete(back);
                    if (File.Exists(file)) File.Move(file, back);
                    File.Move(temp, file);
                }
                else
                {
                    // test file, file incorrect
                }
            }
            else
            {
                // download fail.
            }
        }

        static bool TestFile(string file)
        {
            IPCountryFinder finder = new IPCountryFinder(file);

            // add test case here
            if (finder.GetCountryCode("168.95.1.1") != "TW") return false;


            return true;
        }

        static bool DownloadAndExtractGZip(string url, string file)
        {
//            string rawurl = @"http://software77.net/geo-ip/?DL=1";


            using (var client = new HttpClient())
            {
                HttpResponseMessage rsp = client.GetAsync(url).Result;

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    //File.WriteAllBytes(@"d:\ip2c.csv", rsp.Content.ReadAsByteArrayAsync().Result);
                    Stream source = rsp.Content.ReadAsStreamAsync().Result;

                    GZipStream gzs = new GZipStream(source, CompressionMode.Decompress);
                    FileStream fs = File.OpenWrite(file);

                    int count = 0;
                    byte[] buffer = new byte[4096];
                    while((count = gzs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, count);
                    }
                    gzs.Close();
                    fs.Close();

                    source.Close();

                    return true;
                }
            }


            return false;
        }
    }
}

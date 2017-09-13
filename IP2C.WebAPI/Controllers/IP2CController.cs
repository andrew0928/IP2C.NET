using IP2C.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Hosting;
using System.Web.Http;

namespace IP2C.WebAPI.Controllers
{
    public class IP2CController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return this.GetType().Assembly.GetName().Version.ToString();
        }


        // GET api/values/5
        public object Get(uint id)
        {
            try
            {
                IPCountryFinder ipcf = this.LoadIPDB();

                string ipv4 = this.ConvertIntToIpAddress(id);
                string countryCode = ipcf.GetCountryCode(ipv4);

                return new
                {
                    CountryName = ipcf.ConvertCountryCodeToName(countryCode),
                    CountryCode = countryCode,
                    ServerInfo = new
                    {
                        ClientAddress = System.Web.HttpContext.Current.Request.UserHostAddress,
                        ServerAddress = System.Web.HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"],
                        Version = this.GetType().Assembly.GetName().Version.ToString(),
                        QueryTime = DateTime.Now.ToString("s")
                    }
                };
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        private string ConvertIntToIpAddress(uint ipv4_value)
        {
            //return string.Format(
            //    "{0}.{1}.{2}.{3}",
            //    (ipv4_value / 1000 / 1000 / 1000) % 1000,
            //    (ipv4_value / 1000 / 1000) % 1000,
            //    (ipv4_value / 1000) % 1000,
            //    (ipv4_value) % 1000);

            return string.Format(
                "{0}.{1}.{2}.{3}",
                (ipv4_value >> 24) & 0x00ff,
                (ipv4_value >> 16) & 0x00ff,
                (ipv4_value >> 08) & 0x00ff,
                (ipv4_value >> 00) & 0x00ff);
        }

        private IPCountryFinder LoadIPDB()
        {
            string cachekey = "storage:ip2c";

            IPCountryFinder result = MemoryCache.Default.Get(cachekey) as IPCountryFinder;
            if (result == null)
            {
                string filepath = HostingEnvironment.MapPath("~/App_Data/ipdb.csv");

                try
                {
                    var cip = new CacheItemPolicy();
                    cip.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filepath }));

                    result = new IPCountryFinder(filepath);
                    MemoryCache.Default.Add(
                        cachekey,
                        result,
                        cip);
                }
                catch
                {
                    // can not monitor file

                    result = new IPCountryFinder(filepath);
                    MemoryCache.Default.Add(
                        cachekey,
                        result,
                        DateTimeOffset.Now.AddMinutes(60));
                }
            }

            return result;
        }
    }
}

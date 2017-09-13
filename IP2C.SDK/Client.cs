using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace IP2C.SDK
{
    public class Client : IDisposable
    {
        private HttpClient _connection = null;

        private Version RequiredServerVersion = new Version(3, 1, 0, 0);

        public Client(Uri serviceURL)
        {
            this._connection = new HttpClient();
            this._connection.BaseAddress = serviceURL;

            {
                // 從 3.0.0.0 開始支援 /api/ip2c 這個 API, 會傳回 server version number

                string result = JsonConvert.DeserializeObject<string>(this._connection.GetStringAsync("/api/ip2c").Result);
                Version serverVersion = Version.Parse(result);
                if (serverVersion.Major != this.RequiredServerVersion.Major || serverVersion.Minor < this.RequiredServerVersion.Minor)
                {
                    throw new InvalidOperationException("server version not match the minimal requirement.");
                }
            }
        }

        public void Dispose()
        {
            if (this._connection != null)
            {
                this._connection.Dispose();
            }
        }

        public CountryInfo FindIPCountry(string ipv4_address)
        {
            uint ipv4_value = 0;

            // do ipv4_address format check
            foreach(string value in ipv4_address.Split('.'))
            {
                ipv4_value = (ipv4_value << 8) | uint.Parse(value);
            }

            return this.FindIPCountry(ipv4_value);
        }

        public CountryInfo FindIPCountry(uint ipv4_value)
        {
            string cachekey = string.Format("cache://findip_country/{0}", ipv4_value);
            CountryInfo result = MemoryCache.Default.Get(cachekey) as CountryInfo;

            if (result == null)
            {
                result = this.FindIPCountryWithoutCache(ipv4_value);
                MemoryCache.Default.Add(cachekey, result, new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(60.0),
                    Priority = CacheItemPriority.NotRemovable
                });
            }

            return result;
        }

        private CountryInfo FindIPCountryWithoutCache(uint ipv4_value)
        {
            string result = this._connection.GetStringAsync(string.Format("/api/ip2c/{0}", ipv4_value)).Result;
            return JsonConvert.DeserializeObject<CountryInfo>(result);
        }
    }


    public class CountryInfo
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public ServerInfo ServerInfo { get; set; }
    }

    public class ServerInfo
    {
        public string ClientAddress { get; set; }
        public string ServerAddress { get; set; }
        public string Version { get; set; }
    }
}

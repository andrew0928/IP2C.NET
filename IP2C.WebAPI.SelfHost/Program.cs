using Consul;
using IP2C.WebAPI.Controllers;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace IP2C.WebAPI.SelfHost
{
    class Program
    {
        static string GetLocalIPv4Address(string network =  null)
        {
            // default network region settings: 0.0.0.0/0
            byte[] net_id = new byte[] { 0, 0, 0, 0 };
            byte[] net_mask = new byte[] { 0, 0, 0, 0 };


            //
            // parsing network settings
            //
            if (string.IsNullOrEmpty(network) == false)
            {
                // network format: 0.0.0.0/24
                string[] segments = network.Split('/');
                string[] ipdigits = segments[0].Split('.');
                int mask_size = int.Parse(segments[1]);

                for (int index = 0; index < 4; index++)
                {
                    net_id[index] = byte.Parse(ipdigits[index]);

                    int size = Math.Min(mask_size, 8);
                    mask_size -= size;
                    net_mask[index] = (byte)((0x000000ff << (8-size)) & 0x000000ff);
                }
            }

            //
            //  find matched ip address
            //
            foreach (var ipv4 in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ipv4.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;  // only keep IPv4

                byte[] ipbytes = ipv4.GetAddressBytes();
                bool match = true;
                for (int index = 0; index < 4; index++)
                {
                    if ((net_id[index] & net_mask[index]) != (ipbytes[index] & net_mask[index]))
                    {
                        match = false;
                        break;
                    }
                }

                if (match) return ipv4.ToString();
            }

            //
            //  return loopback ip if not found
            //
            return "127.0.0.1";
        }



        #region shutdown event handler
        private static AutoResetEvent shutdown = new AutoResetEvent(false);

        [DllImport("Kernel32")]
        static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        delegate bool EventHandler(CtrlType sig);
        //static EventHandler _handler;
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static bool ShutdownHandler(CtrlType sig)
        {
            //Console.WriteLine("Shutdown Console Apps...");
            //brs.StopWorkers();
            shutdown.Set();
            Console.WriteLine($"Shutdown WebHost...");
            return true;
        }

        //private static void WaitUntilShutdown()
        //{
        //    SetConsoleCtrlHandler(ShutdownHandler, true);
        //    w.WaitOne();
        //}
        #endregion




        static void Main(string[] args)
        {
            #region parsing argument(s)

            // usage: SelfHost.exe [-network 192.168.100.0/24] [-url http://localhost:9000/] [-consul http://localhost:8500/]

            string local_ip = null;
            string baseAddress = null;
            string consulAddress = null;

            for (int index = 0; index < args.Length; index+=2)
            {
                switch(args[index])
                {
                    case "-network":
                        local_ip = GetLocalIPv4Address(network: args[index + 1]);
                        break;

                    case "-url":
                        baseAddress = args[index + 1];
                        break;

                    case "-consul":
                        consulAddress = args[index + 1];
                        break;

                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(baseAddress) && string.IsNullOrEmpty(local_ip)) local_ip = "127.0.0.1";
            if (string.IsNullOrEmpty(baseAddress)) baseAddress = $"http://{local_ip}:80/";
            #endregion

            #region init windows shutdown handler
            SetConsoleCtrlHandler(ShutdownHandler, true);
            Console.WriteLine($"* Press [CTRL-C] to exit WebAPI-SelfHost...");
            #endregion

            // Start OWIN host 
            Console.WriteLine($"Starting WebApp... (Bind URL: {baseAddress})");
            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine($"WebApp Started.");
                string serviceID = $"IP2CAPI-{Guid.NewGuid():N}".ToUpper(); //Guid.NewGuid().ToString("N");

                // ServiceDiscovery.Register()
                using (ConsulClient consul = new ConsulClient(c => { if (!string.IsNullOrEmpty(consulAddress)) c.Address = new Uri(consulAddress); }))
                {

                    #region register services
                    consul.Agent.ServiceRegister(new AgentServiceRegistration()
                    {
                        Name = "IP2CAPI",
                        ID = serviceID,
                        Address = baseAddress,
                        Checks = new AgentServiceCheck[]
                        {
                            new AgentServiceCheck()
                            {
                                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30.0),
                                HTTP = $"{baseAddress}api/diag/echo/00000000",
                                Interval = TimeSpan.FromSeconds(1.0)
                            },
                            new AgentServiceCheck()
                            {
                                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30.0),
                                TTL = TimeSpan.FromSeconds(5.0)
                            }
                        }
                    }).Wait();
                    #endregion


                    #region send heartbeats to consul
                    bool stop = false;
                    Task heartbeats = Task.Run(() =>
                    {
                        IP2CController ip2c = new IP2CController();
                        while (stop == false)
                        {
                            Task.Delay(1000).Wait();

                            try
                            {
                                var result = ip2c.Get(0x08080808);
                                if (result.CountryCode == "US")
                                {
                                    consul.Agent.PassTTL($"service:{serviceID}:2", $"self test pass.");
                                }
                                else
                                {
                                    consul.Agent.WarnTTL($"service:{serviceID}:2", $"self test fail. query 8.8.8.8, expected: US, actual: {result.CountryCode}({result.CountryName})");
                                }
                            }
                            catch(Exception ex)
                            {
                                consul.Agent.FailTTL($"service:{serviceID}:2", $"self test error. exception: {ex}");
                            }
                        }
                    });

                    // wait until process shutdown (ctrl-c, or close window)
                    shutdown.WaitOne();

                    stop = true;
                    #endregion

                    // ServiceDiscovery.UnRegister()
                    consul.Agent.ServiceDeregister(serviceID).Wait();

                    // wait 5 sec and shutdown owin host
                    Task.Delay(5000).Wait();
                }
            }
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "QueryApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DiagnoisticApi",
                routeTemplate: "api/{controller}/{action}/{text}",
                defaults: new { id = RouteParameter.Optional }
            );

            // do nothing, just force app domain load controller's assembly
            Console.WriteLine($"- Force load controller: {typeof(IP2CController)}");
            Console.WriteLine($"- Force load controller: {typeof(DiagController)}");


            //config.Services.Replace(typeof(IAssembliesResolver), new MyNewAssembliesResolver());

            appBuilder.UseWebApi(config);
        }
    }

    //public class MyNewAssembliesResolver : DefaultAssembliesResolver
    //{
    //    public override ICollection<Assembly> GetAssemblies()
    //    {
    //        Console.WriteLine($"Force load type: {typeof(IP2CController)}");
    //        return base.GetAssemblies();

    //        ICollection<Assembly> baseAssemblies = base.GetAssemblies();
    //        baseAssemblies.Clear();
    //        List<Assembly> assemblies = new List<Assembly>(baseAssemblies);

    //        List<Type> controllers = new List<Type>()
    //        {
    //            typeof(IP2C.WebAPI.Controllers.IP2CController),
    //            typeof(IP2C.WebAPI.Controllers.DiagController)
    //        };

    //        foreach (var controller in controllers)
    //        {
    //            var controllersAssembly = controller.Assembly; // Assembly.LoadFrom(@"Path_to_Controller_DLL");
    //            if (baseAssemblies.Contains(controllersAssembly) == true) continue;
    //            baseAssemblies.Add(controllersAssembly);
    //        }

    //        return baseAssemblies;

    //    }
    //}

}
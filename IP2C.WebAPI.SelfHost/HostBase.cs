using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using Owin;

namespace IP2C.WebAPI.SelfHost
{
    interface IServiceHostStart
    {
        void Configuration(IAppBuilder appBuilder);
    }

    public class ServiceHost : IDisposable
    {
        public ServiceHost()
        {

        }




        public string webapp_baseAddress = "http://127.0.0.1:1234";








        public IDisposable Start<TStartup>() where TStartup : new ()
        {
            {
                SetConsoleCtrlHandler(ShutdownHandler, true);

                _form = new HiddenForm()
                {
                    ShowInTaskbar = false,
                    Visible = false,
                    WindowState = FormWindowState.Minimized
                };

                Task.Run(() =>
                {
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(_form);
                });
            }

            Console.WriteLine($"# Host startup.");

            this._app = WebApp.Start<TStartup>(this.webapp_baseAddress);
            
            // todo: register


            return this;
        }

        private IDisposable _app = null;

        public void Dispose()
        {
            // todo: unregister

            // todo: idle 10 sec

            if (this._app != null) this._app.Dispose();

            Console.WriteLine($"# Host shutdown.");
            SetConsoleCtrlHandler(ShutdownHandler, false);
            _form.Close();


        }

        private void WaitShutdown()
        {
            int index = WaitHandle.WaitAny(new WaitHandle[] {
                _close_program,
                _form.shutdown
            });

            switch(index)
            {
                case 0:
                    Console.WriteLine($"# Receive CTRL-C, CTRL-BREAK or Close Program signal.");
                    break;

                case 1:
                    Console.WriteLine($"# Receive OS shutdown or User logoff signal.");
                    break;
            }
        }

        #region handle ctrl-c, ctrl-break, and close window events
        private static ManualResetEvent _close_program = new ManualResetEvent(false);

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
            _close_program.Set();
            Console.WriteLine($"Shutdown WebHost...");
            return true;
        }

        #endregion


        #region handle logoff or shutdown events
        private static HiddenForm _form = null;
        #endregion

        public static void Main(string[] args)
        {
            var host = new ServiceHost()
            {
                webapp_baseAddress = "http://localhost:9000"
            };

            using (host.Start<Startup>())
            {
                Console.WriteLine($"host started and running...");



                host.WaitShutdown();
                Console.WriteLine($"host is shutting down...");
            }
        }
    }


}

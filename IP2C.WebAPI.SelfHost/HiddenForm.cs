using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IP2C.WebAPI.SelfHost
{
    public partial class HiddenForm : Form
    {
        public HiddenForm()
        {
            InitializeComponent();
        }

        public ManualResetEvent shutdown = new ManualResetEvent(false);

        public Task ShutdownTask = null;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x11) // WM_QUERYENDSESSION
            {
                m.Result = (IntPtr)1;
                Console.WriteLine("winmsg: WM_QUERYENDSESSION");
                this.shutdown.Set();

                // TODO: ugly code here!!!

                // block shutdown process as long as possible until form is closing.
                // max: 10 sec
                while (this._form_closing == false) Thread.SpinWait(100);

                return;
            }

            base.WndProc(ref m);
        }

        private bool _form_closing = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            this._form_closing = true;
        }

    }
}

using Constellation.Package;
using System;
using System.Windows.Forms;

namespace MediaPlayer
{
    class Program : PackageBase
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args = null)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }

        public override void OnShutdown()
        {
            PackageHost.PurgeStateObjects();
            Application.Exit();
        }
    }
}

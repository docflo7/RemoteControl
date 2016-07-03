using Constellation;
using Constellation.Package;
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

namespace MediaPlayer
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            PackageHost.PurgeStateObjects("*", "*");
            PackageHost.RegisterStateObjectLinks(this);
            PackageHost.RegisterMessageCallbacks(this);
            PackageHost.DeclarePackageDescriptor();
            player.MediaChange += new AxWMPLib._WMPOCXEvents_MediaChangeEventHandler(player_MediaChange);

            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            PackageHost.PushStateObject("CurrentSong", new TupleList<string, string, string> { });
            

            this.Text = string.Format("MediaPlayer");
            
        }

    }
}

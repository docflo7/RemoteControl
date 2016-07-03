using Constellation;
using Constellation.Package;
using RemoteControl;
using RemoteControl.PushBullet.MessageCallbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
            Console.WriteLine(args[0]);

            MyConstellation.Packages.Pushbullet.CreatePushBulletScope().SendPush(new SendPushRequest
            {
                Message = $"{args[0]}",
                Title = "Tâche programmée"
            });       
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning : {0} - IsConnected : {1}", PackageHost.IsRunning, PackageHost.IsConnected);            
        }
    }
}

using AsfMojo.Media;
using Constellation.Package;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using RemoteWebcam.PushBullet.MessageCallbacks;

namespace RemoteWebcam
{
    public class Program : PackageBase
    {
        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning : {0} - IsConnected : {1}", PackageHost.IsRunning, PackageHost.IsConnected);
            // initialise la liste des périphériques video et audio
            Collection<EncoderDevice> Vdevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

            foreach (EncoderDevice dev in Vdevices)
            {
                Console.WriteLine(dev.Name);
            }

            Console.WriteLine("Audio :");
            Collection<EncoderDevice> Adevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            foreach (EncoderDevice dev in Adevices)
            {
                Console.WriteLine(dev.Name);
            }
            



        }

        private string VideoCapture(Collection<EncoderDevice> Vdevices)
        {
            // Starts new job for preview window
            LiveJob _job = new LiveJob();
                        
            // Create a new device source. We use the first audio and video devices on the system
            LiveDeviceSource _deviceSource = _job.AddDeviceSource(Vdevices[0], null);

            // Make this source the active one
            _job.ActivateSource(_deviceSource);

            FileArchivePublishFormat fileOut = new FileArchivePublishFormat();
            

            // Sets file path and name
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string output = String.Format(@"{0}\Constellation{1:yyyyMMdd_hhmmss}", path, DateTime.Now);
            fileOut.OutputFileName = string.Format("{0}.wmv",output);
            

            // Adds the format to the job. You can add additional formats
            // as well such as Publishing streams or broadcasting from a port
            _job.PublishFormats.Add(fileOut);
            

            // Starts encoding
            _job.StartEncoding();

            Thread.Sleep(3000);

            _job.StopEncoding();

            _job.RemoveDeviceSource(_deviceSource);
            return output;
        }

        private void ExtractFrame(string input)
        {
            Bitmap bitmap = AsfImage.FromFile(string.Format("{0}.wmv", input), 1.0);
            bitmap.Save(string.Format("{0}.bmp", input));
        }

        private void DeleteSources(string directory)
        {
            System.IO.File.Delete(directory + ".wmv");
            System.IO.File.Delete(directory + ".bmp");
        }

        [MessageCallback]
        void TakePicture(string manufacturer, string model)
        {
            Collection<EncoderDevice> Vdevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);

            foreach (EncoderDevice dev in Vdevices)
            {
                Console.WriteLine(dev.Name);
            }

            Console.WriteLine("Audio :");
            Collection<EncoderDevice> Adevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            foreach (EncoderDevice dev in Adevices)
            {
                Console.WriteLine(dev.Name);
            }

            string path;
            path = VideoCapture(Vdevices);
            ExtractFrame(path);
            string message = string.Format("New pic from your Webcam @ {0}", PackageHost.SentinelName);
            var task = MyConstellation.Packages.Pushbullet.CreatePushBulletScope().GetDevices();
            double updated = 0;
            Device mostRecentMatching = null;
            if (task.Wait(5000) && task.IsCompleted)
            {
                foreach (Device dev in task.Result.Devices)
                {
                    if (dev.Manufacturer == manufacturer && dev.Model == model && dev.IsActive)
                    {
                        if (dev.Modified > updated)
                        {
                            updated = dev.Modified;
                            mostRecentMatching = dev;
                        }
                    }
                }
            }
            if (mostRecentMatching != null)
            {
                MyConstellation.Packages.Pushbullet.CreatePushBulletScope().PushFile(fileUri: path + ".bmp", body: message, target: PushTargetType.Device, targetArgument: mostRecentMatching.Id);
            }
            Thread.Sleep(5000);
            DeleteSources(path);
        }
    }
}

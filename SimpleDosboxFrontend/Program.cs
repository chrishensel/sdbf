using System;
using System.Windows.Forms;
using SimpleDosboxFrontend.Common;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Forms;
using SimpleDosboxFrontend.Platform;
using SimpleDosboxFrontend.Run;

namespace SimpleDosboxFrontend
{
    static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            IPlatformService platformService = new PlatformService();

            if (!platformService.IsCurrentPlatformSupported)
            {
                return 1;
            }

            Ioc.Add<IPlatformService>(platformService);
            platformService.Initialize();

            Ioc.Add<IConfBuilder>(new ConfBuilder());
            Ioc.Add<IIconCreator>(new IconCreator());
            Ioc.Add<IProfileService>(new ProfileService());
            Ioc.Add<IRunService>(new RunService());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            return 0;
        }
    }
}
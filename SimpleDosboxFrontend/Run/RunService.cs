using System.Collections.Generic;
using System.Diagnostics;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Platform;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Run
{
    class RunService : IRunService
    {
        private readonly Dictionary<Process, Profile> _runningProcesses;

        private IPlatformService Platform
        {
            get { return Ioc.Get<IPlatformService>(); }
        }

        private IProfileService ProfileService
        {
            get { return Ioc.Get<IProfileService>(); }
        }


        public RunService()
        {
            _runningProcesses = new Dictionary<Process, Profile>();
        }

        void IService.Initialize()
        {
        }

        void IRunService.Run(Profile profile)
        {
            var confFile = ConfBuilder.GetOrCreateConfFile(profile);

            var psi = new ProcessStartInfo();
            psi.FileName = Platform.GetDosboxPath();
            psi.Arguments = "-conf \"" + confFile.FullName + "\"";

            lock (_runningProcesses)
            {
                var process = new Process();
                process.StartInfo = psi;
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                _runningProcesses.Add(process, profile);

                process.Start();
            }
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            var proc = (Process)sender;
            proc.Exited -= Process_Exited;

            Profile profile;

            lock (_runningProcesses)
            {
                profile = _runningProcesses[proc];

                _runningProcesses.Remove(proc);
            }

            profile.PlayCount++;
            profile.PlayDuration += proc.ExitTime - proc.StartTime;

            ProfileService.Update(profile);
        }
    }
}
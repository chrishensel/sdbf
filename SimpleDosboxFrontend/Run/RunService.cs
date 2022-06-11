using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Platform;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Run
{
    class RunService : IRunService
    {
        private readonly Dictionary<Process, IProfile> _runningProcesses;

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
            _runningProcesses = new Dictionary<Process, IProfile>();
        }

        void IService.Initialize()
        {
        }

        void IRunService.Run(IProfile profile)
        {
            var confFile = Ioc.Get<IConfBuilder>().GetOrCreateConfFile(profile);

            var psi = new ProcessStartInfo
            {
                FileName = Platform.GetDosboxPath(),
                Arguments = "-conf \"" + confFile.FullName + "\""
            };

            if (!File.Exists(psi.FileName))
            {
                return;
            }

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

        private void Process_Exited(object sender, EventArgs e)
        {
            var proc = (Process)sender;
            proc.Exited -= Process_Exited;

            IProfile profile;

            lock (_runningProcesses)
            {
                profile = _runningProcesses[proc];

                _runningProcesses.Remove(proc);
            }

            profile.PlayCount++;
            profile.PlayDuration += proc.ExitTime - proc.StartTime;
            profile.LastPlayedAt = DateTimeOffset.UtcNow;

            ProfileService.Update(profile);
        }
    }
}
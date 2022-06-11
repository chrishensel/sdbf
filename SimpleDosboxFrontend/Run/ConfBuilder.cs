using System;
using System.IO;
using SimpleDosboxFrontend.Data;
using SimpleDosboxFrontend.Platform;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Run
{
    class ConfBuilder : IConfBuilder
    {
        private const string ConfFileExtension = ".conf";

        void IService.Initialize()
        {
        }

        FileInfo IConfBuilder.GetOrCreateConfFile(IProfile profile)
        {
            var confFileName = profile.OriginFile.Directory.FullName;
            confFileName += Path.DirectorySeparatorChar + profile.OriginFile.Name + ConfFileExtension;

            var confFile = new FileInfo(confFileName);

            if (!confFile.Exists
                || confFile.LastWriteTimeUtc < profile.OriginFile.LastWriteTimeUtc)
            {
                try
                {
                    using (var writer = new StreamWriter(confFileName, false))
                    {
                        Write(profile, writer);
                    }
                }
                catch (Exception)
                {
                }
            }

            confFile.Refresh();

            return confFile;
        }

        private static void Write(IProfile profile, TextWriter writer)
        {
            foreach (var sectionName in DosboxConfig.GetSections())
            {
                writer.WriteLine("[{0}]", sectionName);

                foreach (var sectionEntry in DosboxConfig.GetValues(sectionName))
                {
                    var concatenatedKey = string.Format("{0}.{1}", sectionName, sectionEntry.Key);

                    var value = ReplaceKnownVariables(profile.Overrides[concatenatedKey] ?? sectionEntry.Value);

                    writer.WriteLine("{0}={1}", sectionEntry.Key, value);
                }

                writer.WriteLine();
            }

            // Write autoexec
            writer.WriteLine("[autoexec]");
            writer.WriteLine("@echo off");
            writer.WriteLine("SET PATH=Z:/");
            writer.WriteLine("keyb GR 437");

            foreach (var path in profile.Paths)
            {
                writer.WriteLine("mount " + path.MountAs + " \"" + path.OSPath.FullName + "\"");
            }

            writer.WriteLine("echo.");
            writer.WriteLine("C:");

            writer.WriteLine(profile.RunInfo.Command);

            if (profile.RunInfo.AutoExit)
            {
                writer.WriteLine("exit");
            }
        }

        private static string ReplaceKnownVariables(string value)
        {
            var platform = Ioc.Get<IPlatformService>();

            return value
                .Replace("$ScreenSize$",
                    string.Format("{0}x{1}", platform.GetScreenSize().Width, platform.GetScreenSize().Height));
        }
    }
}

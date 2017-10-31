using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SimpleDosboxFrontend.Properties;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Platform
{
    class PlatformService : IPlatformService
    {
        private const string UserDataDirectoryName = "SimpleDosboxFrontend";
        private const string ProfileDirectoryName = "profiles";

        private const char PathIndicatorVerbatim = '!';
        private const char PathIndicatorRelativeToApplication = '@';
        private const char PathIndicatorRelativeToProfilesDir = '$';

        private readonly SupportedPlatform _platform;
        private readonly DirectoryInfo _applicationDir;
        private DirectoryInfo _userDataDir;
        private DirectoryInfo _profilesDir;

        public PlatformService()
        {
            _applicationDir = new DirectoryInfo(Application.StartupPath);

            var os = Environment.OSVersion;

            if (os.Platform == PlatformID.Unix)
            {
                _platform = SupportedPlatform.Unix;
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                _platform = SupportedPlatform.Windows;
            }
        }

        void IService.Initialize()
        {
            var basePath = string.Empty;
            var profilesPath = string.Empty;

            if (Settings.Default.PortableMode)
            {
                basePath = _applicationDir.FullName;

                profilesPath = Path.Combine(basePath, Settings.Default.PortableModeProfileDir);
            }
            else
            {
                basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                basePath = Path.Combine(basePath, UserDataDirectoryName);

                profilesPath = Path.Combine(basePath, ProfileDirectoryName);
            }

            _userDataDir = new DirectoryInfo(basePath);
            _profilesDir = new DirectoryInfo(profilesPath);

            if (!_userDataDir.Exists)
            {
                _userDataDir.Create();
            }

            if (!_profilesDir.Exists)
            {
                _profilesDir.Create();
            }

            _userDataDir.Refresh();
            _profilesDir.Refresh();
        }

        bool IPlatformService.IsCurrentPlatformSupported
        {
            get { return _platform != SupportedPlatform.Unsupported; }
        }

        DirectoryInfo IPlatformService.ProfilesDirectory
        {
            get { return _profilesDir; }
        }

        string IPlatformService.GetPlatformPath(string monikerPath)
        {
            if (monikerPath?.Length < 2)
            {
                throw new ArgumentException(Resources.InvalidMonikerPathMessage, nameof(monikerPath));
            }

            var pathIndicator = monikerPath[0];
            var pathWithoutIndicator = monikerPath.Remove(0, 1);
            var platformPath = string.Empty;

            switch (pathIndicator)
            {
                case PathIndicatorVerbatim:
                    // OS-specific, verbatim, absolute.
                    platformPath = pathWithoutIndicator;
                    break;

                case PathIndicatorRelativeToApplication:
                    // Relative to the application directory.
                    platformPath = GetRelativePlatformPath(pathWithoutIndicator, _applicationDir.FullName);
                    break;

                case PathIndicatorRelativeToProfilesDir:
                    // Relative to the profiles directory.
                    platformPath = GetRelativePlatformPath(pathWithoutIndicator, _profilesDir.FullName);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return platformPath;
        }

        private static string GetRelativePlatformPath(string monikerPath, string basePath)
        {
            string platformPath = string.Empty;

            // Equalize windows and unix paths by replacing the separator with a forward slash.
            var sanitized = monikerPath.Replace(Path.DirectorySeparatorChar, '/');
            var tokens = sanitized.Split('/').ToList();

            tokens.Insert(0, basePath);

            foreach (var token in tokens)
            {
                platformPath = Path.Combine(platformPath, token);
            }

            return platformPath;
        }

        string IPlatformService.GetDosboxPath()
        {
            string dosbox = null;

            switch (_platform)
            {
                case SupportedPlatform.Windows:
                    dosbox = new FileInfo(Settings.Default.DOSBoxExecutablePathWindows).FullName;
                    break;
                case SupportedPlatform.Unix:
                    dosbox = Settings.Default.DOSBoxExecutablePathUnix;
                    break;
            }

            return dosbox;
        }

        Size IPlatformService.GetScreenSize()
        {
            return Screen.PrimaryScreen.Bounds.Size;
        }

        enum SupportedPlatform
        {
            Unsupported = 0,
            Windows = 1,
            Unix = 2
        }
    }
}
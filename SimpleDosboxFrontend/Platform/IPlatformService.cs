using System.Drawing;
using System.IO;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Platform
{
    interface IPlatformService : IService
    {
        bool IsCurrentPlatformSupported { get; }
        DirectoryInfo ProfilesDirectory { get; }

        /// <summary>
        /// Translates the provided moniker path (an abstract path, file or directory), into an OS-specific path.
        /// </summary>
        /// <returns>The platform path.</returns>
        /// <param name="monikerPath">The moniker path (like '!/opt/dos/').</param>
        string GetPlatformPath(string monikerPath);
        string GetDosboxPath();
        Size GetScreenSize();
    }
}
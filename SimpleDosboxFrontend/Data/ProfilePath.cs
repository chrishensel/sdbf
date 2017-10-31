using System.IO;

namespace SimpleDosboxFrontend.Data
{
    class ProfilePath
    {
        public char MountAs { get; set; }
        public string OriginalPath { get; set; }
        public DirectoryInfo OSPath { get; set; }
    }
}
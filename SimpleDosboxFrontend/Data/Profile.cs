using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace SimpleDosboxFrontend.Data
{
    class Profile
    {
        public FileInfo OriginFile { get; set; }
        public string Name { get; set; }
        public string Developer { get; set; }
        public string Description { get; set; }
        public int PlayCount { get; set; }
        public TimeSpan PlayDuration { get; set; }
        public IList<ProfilePath> Paths { get; set; }
        public ProfileRunInfo RunInfo { get; set; }
        /// <summary>
        /// Gets/sets the key and values for DOSBox settings to override.
        /// These will be applied as the last step, after specialized settings have been set.
        /// </summary>
        public NameValueCollection Overrides { get; set; }

        public Profile()
        {
            Paths = new List<ProfilePath>();
            RunInfo = new ProfileRunInfo();
            Overrides = new NameValueCollection();
        }
    }
}
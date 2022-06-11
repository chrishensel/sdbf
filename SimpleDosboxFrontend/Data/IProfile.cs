using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace SimpleDosboxFrontend.Data
{
    interface IProfile
    {
        FileInfo OriginFile { get; set; }
        string Name { get; set; }
        string Developer { get; set; }
        string Description { get; set; }
        int PlayCount { get; set; }
        TimeSpan PlayDuration { get; set; }
        DateTimeOffset LastPlayedAt { get; set; }
        IList<ProfilePath> Paths { get; set; }
        ProfileRunInfo RunInfo { get; set; }
        /// <summary>
        /// Gets/sets the key and values for DOSBox settings to override.
        /// These will be applied as the last step, after specialized settings have been set.
        /// </summary>
        NameValueCollection Overrides { get; set; }
    }
}

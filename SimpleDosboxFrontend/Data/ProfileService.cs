using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SimpleDosboxFrontend.Common;
using SimpleDosboxFrontend.Platform;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Data
{
    class ProfileService : IProfileService
    {
        private const string ProfileExtension = ".sdbpx";
        private readonly FileInfo _profilesMetadataFile;

        private HashSet<Profile> _loadedProfiles;

        public event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated;

        private IPlatformService PlatformService
        {
            get { return Ioc.Get<IPlatformService>(); }
        }

        public ProfileService()
        {
            _loadedProfiles = new HashSet<Profile>();

            var profilesMetadataFilePath = Path.Combine(PlatformService.ProfilesDirectory.FullName, "Profiles.Metadata.xml");
            _profilesMetadataFile = new FileInfo(profilesMetadataFilePath);

            RescanProfiles();
        }

        private void OnProfileUpdated(Profile profile)
        {
            ProfileUpdated?.Invoke(this, new ProfileUpdatedEventArgs(profile));
        }

        private void RescanProfiles()
        {
            _loadedProfiles.Clear();

            XDocument docMetadata = null;

            if (_profilesMetadataFile.Exists)
            {
                docMetadata = XDocument.Load(_profilesMetadataFile.FullName);
            }

            foreach (var file in PlatformService.ProfilesDirectory.EnumerateFiles("*" + ProfileExtension, SearchOption.AllDirectories))
            {
                try
                {
                    XDocument doc = null;

                    using (var stream = file.OpenRead())
                    {
                        doc = XDocument.Load(stream);
                    }

                    var profile = DeserializeProfile(doc);
                    profile.OriginFile = file;

                    ApplyMetadataToProfile(docMetadata, profile);

                    _loadedProfiles.Add(profile);
                }
                catch (Exception)
                {

                }
            }
        }

        void IService.Initialize()
        {
        }

        IEnumerable<Profile> IProfileService.GetProfiles()
        {
            return _loadedProfiles.OrderBy(_ => _.Name);
        }

        void IProfileService.Update(Profile profile)
        {
            var exists = _profilesMetadataFile.Exists;

            XDocument infoDoc;

            if (exists)
            {
                infoDoc = XDocument.Load(_profilesMetadataFile.FullName);
            }
            else
            {
                infoDoc = new XDocument();
                infoDoc.Add(new XElement("profiles-metadata"));
            }

            var game = infoDoc.Root.Elements("profile").FirstOrDefault(_ => _.Attribute("name").Value == profile.Name);
            var gamePlayCount = game?.Element("play-count");
            var gamePlayDuration = game?.Element("play-duration");

            if (game == null)
            {
                game = new XElement("profile");
                game.Add(new XAttribute("name", profile.Name));
                gamePlayCount = new XElement("play-count");
                game.Add(gamePlayCount);
                gamePlayDuration = new XElement("play-duration");
                game.Add(gamePlayDuration);
                infoDoc.Root.Add(game);
            }

            gamePlayCount.Value = profile.PlayCount.ToString();
            gamePlayDuration.Value = ((long)profile.PlayDuration.TotalSeconds).ToString();

            infoDoc.Save(_profilesMetadataFile.FullName);

            OnProfileUpdated(profile);
        }

        private static Profile DeserializeProfile(XDocument xml)
        {
            var root = xml.Root;

            var profile = new Profile();

            // Generic information
            profile.Name = root.TryGetElementValue("name", string.Empty);
            profile.Developer = root.TryGetElementValue("developer", string.Empty);
            profile.Description = root.TryGetElementValue("description", string.Empty);

            // Paths
            var pathsRoot = root.Element("paths");

            if (pathsRoot != null)
            {
                foreach (var pathDeclaration in pathsRoot.Elements("path"))
                {
                    var path = new ProfilePath();
                    path.MountAs = pathDeclaration.TryGetAttributeValue("mount-as", char.MinValue);
                    path.OriginalPath = pathDeclaration.Value;

                    InterpretPathOS(path);
                    profile.Paths.Add(path);
                }
            }

            // Run config
            var runRoot = root.Element("run");

            if (runRoot != null)
            {
                var autoExit = runRoot.TryGetAttributeValue("auto-exit", true);
                var command = runRoot.Value;

                profile.RunInfo.AutoExit = autoExit;
                profile.RunInfo.Command = command;
            }

            // Overrides
            var overridesRoot = root.Element("overrides");

            if (overridesRoot != null)
            {
                foreach (var item in overridesRoot.Elements("item"))
                {
                    var name = item.Attribute("name")?.Value;

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    var value = item.Value;

                    profile.Overrides.Set(name, value);
                }
            }

            return profile;
        }

        private void ApplyMetadataToProfile(XDocument xml, Profile profile)
        {
            if (xml == null)
            {
                return;
            }

            var item = xml.Root.Elements("profile").FirstOrDefault(_ => _.Attribute("name").Value == profile.Name);

            if (item == null)
            {
                return;
            }

            profile.PlayCount = item.TryGetElementValue("play-count", 0);

            var playDuration = Extensions.TryGetElementValue(item, "play-duration", 0L);
            profile.PlayDuration = TimeSpan.FromSeconds(playDuration);
        }

        private static void InterpretPathOS(ProfilePath path)
        {
            var platformService = Ioc.Get<IPlatformService>();
            var osPath = platformService.GetPlatformPath(path.OriginalPath);

            path.OSPath = new DirectoryInfo(osPath);
        }

        Image IProfileService.GetProfileImage(Profile profile)
        {
            var imagePath = profile.OriginFile.FullName + ".png";
            var file = new FileInfo(imagePath);

            Image image = null;

            if (!file.Exists)
            {
                image = IconCreator.CreateGenericImage(profile);

                try
                {
                    using (var stream = file.OpenWrite())
                    {
                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    file.Refresh();
                }
                catch (Exception)
                {
                }
            }

            if (file.Exists)
            {
                using (var stream = file.OpenRead())
                {
                    image = Image.FromStream(stream);
                }
            }

            return image;
        }

        Image IProfileService.GetPreviewImage(Profile profile)
        {
            // Capture images are currently only loaded from the "capture" directory.
            var platformService = Ioc.Get<IPlatformService>();

            var capturesDir = platformService.ProfilesDirectory.EnumerateDirectories().FirstOrDefault(_ => string.Equals("capture", _.Name, StringComparison.OrdinalIgnoreCase));

            var fileNameToSearch = profile.RunInfo.Command;

            if (capturesDir == null || string.IsNullOrWhiteSpace(fileNameToSearch))
            {
                return null;
            }

            var exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameToSearch);
            var imageFile = capturesDir
                .EnumerateFiles("*.png", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(_ => _.Name.IndexOf(exeFileNameWithoutExtension, StringComparison.OrdinalIgnoreCase) > -1);

            if (imageFile == null)
            {
                return null;
            }

            return Image.FromFile(imageFile.FullName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Data
{
    interface IProfileService : IService
    {
        event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated;

        IEnumerable<IProfile> GetProfiles();
        void Update(IProfile profile);
        Image GetProfileImage(IProfile profile);
        Image GetPreviewImage(IProfile profile);
    }
}
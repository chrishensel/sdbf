using System;
using System.Collections.Generic;
using System.Drawing;
using SimpleDosboxFrontend.Services;

namespace SimpleDosboxFrontend.Data
{
    interface IProfileService : IService
    {
        event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated;

        IEnumerable<Profile> GetProfiles();
        void Update(Profile profile);
        Image GetProfileImage(Profile profile);
        Image GetPreviewImage(Profile profile);
    }
}
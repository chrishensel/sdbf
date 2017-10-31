using System;

namespace SimpleDosboxFrontend.Data
{
    class ProfileUpdatedEventArgs : EventArgs
    {
    	public Profile Profile { get; private set; }

        public ProfileUpdatedEventArgs(Profile profile)
        {
            Profile = profile;
        }
    }
}
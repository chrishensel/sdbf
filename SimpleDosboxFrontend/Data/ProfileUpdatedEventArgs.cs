using System;

namespace SimpleDosboxFrontend.Data
{
    class ProfileUpdatedEventArgs : EventArgs
    {
    	public IProfile Profile { get; private set; }

        public ProfileUpdatedEventArgs(IProfile profile)
        {
            Profile = profile;
        }
    }
}
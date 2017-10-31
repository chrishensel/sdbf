namespace SimpleDosboxFrontend.Data
{
    class ProfileRunInfo
    {
        public string Command { get; set; }
        public bool AutoExit { get; set; }

        public ProfileRunInfo()
        {
            AutoExit = true;
        }
    }
}
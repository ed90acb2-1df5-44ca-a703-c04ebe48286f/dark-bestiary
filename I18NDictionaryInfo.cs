namespace DarkBestiary
{
    public struct I18NDictionaryInfo
    {
        public string Name { get; }
        public string DisplayName { get; }

        public I18NDictionaryInfo(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
namespace DarkBestiary.Messaging
{
    public struct CharacterCreationEventData
    {
        public string Name { get; }
        public bool IsHardcore { get; }
        public bool IsRandomSkills { get; }

        public CharacterCreationEventData(string name, bool isHardcore, bool isRandomSkills)
        {
            Name = name;
            IsHardcore = isHardcore;
            IsRandomSkills = isRandomSkills;
        }
    }
}
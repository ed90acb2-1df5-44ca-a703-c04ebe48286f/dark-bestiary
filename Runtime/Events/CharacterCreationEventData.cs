namespace DarkBestiary.Events
{
    public struct CharacterCreationEventData
    {
        public string Name { get; }

        public CharacterCreationEventData(string name)
        {
            Name = name;
        }
    }
}
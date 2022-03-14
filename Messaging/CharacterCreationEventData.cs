namespace DarkBestiary.Messaging
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
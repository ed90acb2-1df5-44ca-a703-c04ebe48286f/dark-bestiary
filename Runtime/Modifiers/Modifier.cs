namespace DarkBestiary.Modifiers
{
    public abstract class Modifier<T>
    {
        public ModifierType Type { get; }

        protected Modifier(ModifierType type)
        {
            Type = type;
        }

        public abstract T Modify(T value);
    }
}
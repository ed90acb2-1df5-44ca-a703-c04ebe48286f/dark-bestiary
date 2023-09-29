namespace DarkBestiary.Console
{
    public interface IConsoleCommand
    {
        string GetSignature();

        string GetDescription();

        string Execute(string input);
    }
}
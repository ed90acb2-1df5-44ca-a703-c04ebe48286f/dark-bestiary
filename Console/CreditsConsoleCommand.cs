namespace DarkBestiary.Console
{
    public class CreditsConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "credits";
        }

        public string GetDescription()
        {
            return "Run credits scene.";
        }

        public string Execute(string input)
        {
            Game.Instance.ToOutro();
            return "okay";
        }
    }
}
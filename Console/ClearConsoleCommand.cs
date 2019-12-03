using DarkBestiary.UI.Views;

namespace DarkBestiary.Console
{
    public class ClearConsoleCommand : IConsoleCommand
    {
        private readonly IDeveloperConsoleView view;

        public ClearConsoleCommand(IDeveloperConsoleView view)
        {
            this.view = view;
        }

        public string GetSignature()
        {
            return "clear";
        }

        public string GetDescription()
        {
            return "Clears console.";
        }

        public string Execute(string input)
        {
            this.view.Clear();
            return "";
        }
    }
}
using DarkBestiary.UI.Views;

namespace DarkBestiary.Console
{
    public class ClearConsoleCommand : IConsoleCommand
    {
        private readonly IDeveloperConsoleView m_View;

        public ClearConsoleCommand(IDeveloperConsoleView view)
        {
            m_View = view;
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
            m_View.Clear();
            return "";
        }
    }
}
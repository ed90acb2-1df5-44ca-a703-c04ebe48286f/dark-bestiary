using DarkBestiary.UI.Controllers;

namespace DarkBestiary.Console
{
    public class HelpConsoleCommand : IConsoleCommand
    {
        private readonly DeveloperConsoleViewController m_Controller;

        public HelpConsoleCommand(DeveloperConsoleViewController controller)
        {
            m_Controller = controller;
        }

        public string GetSignature()
        {
            return "help";
        }

        public string GetDescription()
        {
            return "Displays list of available commands.";
        }

        public string Execute(string input)
        {
            m_Controller.DisplayCommands();
            return "";
        }
    }
}
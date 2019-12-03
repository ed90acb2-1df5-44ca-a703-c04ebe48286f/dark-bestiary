using DarkBestiary.UI.Controllers;

namespace DarkBestiary.Console
{
    public class HelpConsoleCommand : IConsoleCommand
    {
        private readonly DeveloperConsoleViewController controller;

        public HelpConsoleCommand(DeveloperConsoleViewController controller)
        {
            this.controller = controller;
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
            this.controller.DisplayCommands();
            return "";
        }
    }
}
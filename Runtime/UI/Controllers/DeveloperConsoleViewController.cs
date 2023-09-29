using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Console;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class DeveloperConsoleViewController : ViewController<IDeveloperConsoleView>
    {
        private readonly List<IConsoleCommand> m_Commands;

        public DeveloperConsoleViewController(IDeveloperConsoleView view) : base(view)
        {
            m_Commands = new List<IConsoleCommand>
            {
                new HelpConsoleCommand(this),
                new ClearConsoleCommand(view),
                new NextRoundConsoleCommand(),
                new ResetLevelConsoleCommand(),
                new KillAllConsoleCommand(),
                Container.Instance.Instantiate<AddRunesConsoleCommand>(),
                Container.Instance.Instantiate<AddAttributePointsConsoleCommand>(),
                Container.Instance.Instantiate<AddTalentPointsConsoleCommand>(),
                Container.Instance.Instantiate<StartScenarioConsoleCommand>(),
                Container.Instance.Instantiate<LevelupConsoleCommand>(),
                Container.Instance.Instantiate<GiveRewardConsoleCommand>(),
                Container.Instance.Instantiate<SpawnUnitConsoleCommand>(),
                Container.Instance.Instantiate<AddSkillConsoleCommand>(),
                Container.Instance.Instantiate<RemoveSkillConsoleCommand>(),
                Container.Instance.Instantiate<ResetCooldownsConsoleCommand>(),
                Container.Instance.Instantiate<AddExperienceConsoleCommand>(),
                Container.Instance.Instantiate<AddItemConsoleCommand>(),
                Container.Instance.Instantiate<AddGemsConsoleCommand>(),
                Container.Instance.Instantiate<AddIngredientsConsoleCommand>(),
                Container.Instance.Instantiate<AddVisionItemsConsoleCommand>(),
            };
        }

        protected override void OnInitialize()
        {
            View.SubmittingCommand += OnSubmittingCommand;
            View.SuggestingCommand += OnSuggestingCommand;
        }

        protected override void OnTerminate()
        {
            View.SubmittingCommand -= OnSubmittingCommand;
            View.SuggestingCommand -= OnSuggestingCommand;
        }

        private void OnSuggestingCommand(string input)
        {
            var signature = input.Split().First();
            var suggested = m_Commands.Where(command => command.GetSignature().StartsWith(signature)).ToList();

            if (suggested.Count == 0)
            {
                View.Info("No commands found.");
                return;
            }

            DisplayCommands(suggested);
        }

        private void OnSubmittingCommand(string input)
        {
            var signature = input.Split().First();
            var command = m_Commands.FirstOrDefault(element => element.GetSignature() == signature);

            if (command == null)
            {
                View.Error($"No command with signature {signature}");
                return;
            }

            ExecuteCommand(command, input.Replace(signature, "").Trim());
        }

        private void ExecuteCommand(IConsoleCommand command, string options)
        {
            var message = command.Execute(options);

            if (message.Length > 0)
            {
                View.Success(message);
            }
        }

        public void DisplayCommands()
        {
            DisplayCommands(m_Commands);
        }

        private void DisplayCommands(IEnumerable<IConsoleCommand> commands)
        {
            View.Info("Commands:");

            foreach (var command in commands)
            {
                View.Info("    " + command.GetSignature() + " - " + command.GetDescription());
            }
        }
    }
}
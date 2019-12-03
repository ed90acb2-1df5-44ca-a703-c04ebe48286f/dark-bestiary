using DarkBestiary.Data.Repositories;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class StartScenarioConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly IScenarioRepository scenarioRepository;

        public StartScenarioConsoleCommand(CharacterManager characterManager, IScenarioRepository scenarioRepository)
        {
            this.characterManager = characterManager;
            this.scenarioRepository = scenarioRepository;
        }

        public string GetSignature()
        {
            return "scenario";
        }

        public string GetDescription()
        {
            return "Start scenario. (Format: [scenarioId]";
        }

        public string Execute(string input)
        {
            var scenario = this.scenarioRepository.FindOrFail(int.Parse(input.Split()[0]));

            Game.Instance.SwitchState(new ScenarioGameState(scenario, this.characterManager.Character));

            return $"Scenario \"{scenario.Name}\" started!";
        }
    }
}
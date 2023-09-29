namespace DarkBestiary.Console
{
    public class StartScenarioConsoleCommand : IConsoleCommand
    {
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
            var scenarioId = int.Parse(input.Split()[0]);

            Game.Instance.ToScenario(scenarioId);

            return $"Scenario \"{scenarioId.ToString()}\" started!";
        }
    }
}
using System;
using DarkBestiary.Scenarios.Encounters;

namespace DarkBestiary.Console
{
    public class NextRoundConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "next_round";
        }

        public string GetDescription()
        {
            return "Skip remaining turns and start new round.";
        }

        public string Execute(string input)
        {
            if (CombatEncounter.Active == null)
            {
                throw new Exception("Not in combat.");
            }

            CombatEncounter.Active.NextRound();

            return "Next round!";
        }
    }
}
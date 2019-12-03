using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.Console
{
    public class SpawnUnitConsoleCommand : IConsoleCommand
    {
        private readonly IUnitRepository unitRepository;
        private readonly BoardNavigator boardNavigator;

        public SpawnUnitConsoleCommand(IUnitRepository unitRepository, BoardNavigator boardNavigator)
        {
            this.unitRepository = unitRepository;
            this.boardNavigator = boardNavigator;
        }

        public string GetSignature()
        {
            return "spawn";
        }

        public string GetDescription()
        {
            return "Spawn unit at random location by id (Format: [unitId])";
        }

        public string Execute(string input)
        {
            var point = this.boardNavigator
                .Walkable()
                .Where(cell => !cell.IsOccupied).Shuffle()
                .First().transform.position;

            var unit = this.unitRepository.Find(int.Parse(input.Split()[0])).GetComponent<UnitComponent>();
            unit.transform.position = point;
            unit.Owner = Owner.Hostile;
            unit.TeamId = 2;

            return $"{unit.name} successfully created at point {point}";
        }
    }
}
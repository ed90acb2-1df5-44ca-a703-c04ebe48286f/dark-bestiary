using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.Console
{
    public class SpawnUnitConsoleCommand : IConsoleCommand
    {
        private readonly IUnitRepository m_UnitRepository;
        private readonly BoardNavigator m_BoardNavigator;

        public SpawnUnitConsoleCommand(IUnitRepository unitRepository, BoardNavigator boardNavigator)
        {
            m_UnitRepository = unitRepository;
            m_BoardNavigator = boardNavigator;
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
            var point = m_BoardNavigator
                .Walkable()
                .Where(cell => !cell.IsOccupied).Shuffle()
                .First().transform.position;

            var unit = m_UnitRepository.Find(int.Parse(input.Split()[0])).GetComponent<UnitComponent>();
            unit.transform.position = point;
            unit.Owner = Owner.Hostile;
            unit.TeamId = 2;

            return $"{unit.name} successfully created at point {point}";
        }
    }
}
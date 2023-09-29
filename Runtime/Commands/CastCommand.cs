using System;
using DarkBestiary.Exceptions;
using DarkBestiary.GameBoard;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.Commands
{
    public class CastCommand : ICommand
    {
        public event Action<ICommand> Done;

        private readonly Skill m_Skill;
        private readonly BoardCell m_Cell;

        public CastCommand(Skill skill, BoardCell cell)
        {
            m_Skill = skill;
            m_Cell = cell;
        }

        public void Execute()
        {
            try
            {
                m_Skill.UseStrategy.Use(m_Skill, m_Cell);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }

            Done?.Invoke(this);
        }
    }
}
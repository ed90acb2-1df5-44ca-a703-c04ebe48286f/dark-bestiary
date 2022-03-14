using DarkBestiary.Exceptions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.Commands
{
    public class CastCommand : ICommand
    {
        public event Payload<ICommand> Done;

        private readonly Skill skill;
        private readonly BoardCell cell;

        public CastCommand(Skill skill, BoardCell cell)
        {
            this.skill = skill;
            this.cell = cell;
        }

        public void Execute()
        {
            try
            {
                this.skill.UseStrategy.Use(this.skill, this.cell);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }

            Done?.Invoke(this);
        }
    }
}
using DarkBestiary.Messaging;

namespace DarkBestiary.Commands
{
    public interface ICommand
    {
        event Payload<ICommand> Done;

        void Execute();
    }
}
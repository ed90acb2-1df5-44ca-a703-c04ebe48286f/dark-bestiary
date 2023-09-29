using System;

namespace DarkBestiary.Commands
{
    public interface ICommand
    {
        event Action<ICommand> Done;

        void Execute();
    }
}
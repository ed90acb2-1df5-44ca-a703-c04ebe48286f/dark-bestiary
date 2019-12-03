using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Commands
{
    public class CommandSequence
    {
        public bool IsDone => this.commands.Count == 0;

        private readonly List<ICommand> commands = new List<ICommand>();

        public CommandSequence Add(ICommand command)
        {
            this.commands.Add(command);
            return this;
        }

        public CommandSequence Start()
        {
            if (this.commands.Count == 0)
            {
                return this;
            }

            StartNext();
            return this;
        }

        private void StartNext()
        {
            var command = this.commands.First();

            command.Done += OnCommandDone;
            command.Execute();

        }

        private void OnCommandDone(ICommand command)
        {
            this.commands.Remove(command);

            if (this.commands.Count == 0)
            {
                return;
            }

            StartNext();
        }
    }
}
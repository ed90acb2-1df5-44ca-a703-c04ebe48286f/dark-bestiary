using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Commands
{
    public class CommandSequence
    {
        public bool IsDone => m_Commands.Count == 0;

        private readonly List<ICommand> m_Commands = new();

        public CommandSequence Add(ICommand command)
        {
            m_Commands.Add(command);
            return this;
        }

        public CommandSequence Start()
        {
            if (m_Commands.Count == 0)
            {
                return this;
            }

            StartNext();
            return this;
        }

        private void StartNext()
        {
            var command = m_Commands.First();

            command.Done += OnCommandDone;
            command.Execute();

        }

        private void OnCommandDone(ICommand command)
        {
            m_Commands.Remove(command);

            if (m_Commands.Count == 0)
            {
                return;
            }

            StartNext();
        }
    }
}
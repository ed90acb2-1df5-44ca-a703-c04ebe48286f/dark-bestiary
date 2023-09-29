using System;

namespace DarkBestiary.Exceptions
{
    public class GameplayException : Exception
    {
        public GameplayException(string message) : base(message)
        {
        }
    }
}
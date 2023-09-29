using System;

namespace DarkBestiary.UI.Views
{
    public interface IDeveloperConsoleView : IView
    {
        event Action<string> SuggestingCommand;
        event Action<string> SubmittingCommand;

        void Info(string text);
        void Error(string text);
        void Success(string text);
        void Clear();
    }
}
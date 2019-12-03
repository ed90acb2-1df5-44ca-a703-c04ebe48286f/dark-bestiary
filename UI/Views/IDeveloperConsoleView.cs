using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IDeveloperConsoleView : IView
    {
        event Payload<string> SuggestingCommand;
        event Payload<string> SubmittingCommand;

        void Info(string text);

        void Error(string text);

        void Success(string text);

        void Clear();
    }
}
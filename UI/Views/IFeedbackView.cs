using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public struct FeedbackRequest
    {
        public string Subject { get; }
        public string Title { get; }
        public string Text { get; }

        public FeedbackRequest(string subject, string title, string text)
        {
            Subject = subject;
            Title = title;
            Text = text;
        }
    }

    public interface IFeedbackView : IView, IHideOnEscape
    {
        event Payload<FeedbackRequest> Submitted;

        void SetFormFieldsInteractable(bool interactable);

        void ClearFormFields();
    }
}
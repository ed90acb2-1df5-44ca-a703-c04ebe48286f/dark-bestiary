using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IVisionSummaryView : IView, IFullscreenView
    {
        event Payload CompleteButtonClicked;

        void Construct(List<Item> rewards, IEnumerable<KeyValuePair<string, string>> summary);
        void SetSuccess(bool isSuccess);
    }
}
using System;
using System.Collections.Generic;

namespace DarkBestiary.UI.Views
{
    public interface IVisionSummaryView : IView, IFullscreenView
    {
        event Action CompleteButtonClicked;

        void Construct(IEnumerable<KeyValuePair<string, string>> summary);
        void SetSuccess(bool isSuccess);
    }
}
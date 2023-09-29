using System;
using System.Collections.Generic;

namespace DarkBestiary.UI.Views
{
    public interface IKeyBindingsView : IView, IHideOnEscape
    {
        event Action<IEnumerable<KeyBindingInfo>> Applied;
        event Action Cancelled;
        event Action Reseted;

        void Construct(IEnumerable<KeyBindingInfo> keyBindingsInfo);
        void Refresh(IEnumerable<KeyBindingInfo> keyBindingsInfo);
    }
}
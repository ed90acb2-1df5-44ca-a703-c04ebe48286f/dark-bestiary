using System.Collections.Generic;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IKeyBindingsView : IView, IHideOnEscape
    {
        event Payload<IEnumerable<KeyBindingInfo>> Applied;
        event Payload Cancelled;
        event Payload Reseted;

        void Construct(IEnumerable<KeyBindingInfo> keyBindingsInfo);
        void Refresh(IEnumerable<KeyBindingInfo> keyBindingsInfo);
    }
}
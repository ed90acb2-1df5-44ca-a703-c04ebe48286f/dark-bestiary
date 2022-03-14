using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;
using DarkBestiary.Utility;

namespace DarkBestiary.UI.Controllers
{
    public class KeyBindingsViewController : ViewController<IKeyBindingsView>
    {
        public KeyBindingsViewController(IKeyBindingsView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Reseted += OnReseted;
            View.Cancelled += OnCancel;
            View.Applied += OnApply;
            View.Construct(GetKeyBindingsInfo());
        }

        protected override void OnTerminate()
        {
            View.Reseted -= OnReseted;
            View.Cancelled -= OnCancel;
            View.Applied -= OnApply;
        }

        private void RefreshView()
        {
            View.Refresh(GetKeyBindingsInfo());
        }

        private IEnumerable<KeyBindingInfo> GetKeyBindingsInfo()
        {
            return KeyBindings.All()
                .Select(keyBinding => new KeyBindingInfo(
                    keyBinding.Key,
                    keyBinding.Value,
                    EnumTranslator.Translate(keyBinding.Key))
                );
        }

        private void OnApply(IEnumerable<KeyBindingInfo> keyBindingsInfo)
        {
            KeyBindings.Change(keyBindingsInfo.ToDictionary(info => info.Type, info => info.Code));
            View.Hide();
        }

        private void OnCancel()
        {
            RefreshView();
            View.Hide();
        }

        private void OnReseted()
        {
            KeyBindings.Reset();
            RefreshView();
        }
    }
}
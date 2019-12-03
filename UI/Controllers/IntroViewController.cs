using System.Collections.Generic;
using DarkBestiary.GameStates;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class IntroViewController : ViewController<IIntroView>
    {
        private readonly Queue<string> texts;

        public IntroViewController(IIntroView view) : base(view)
        {
            this.texts = new Queue<string>();
            this.texts.Enqueue(I18N.Instance.Get("ui_intro_text_1"));
            this.texts.Enqueue(I18N.Instance.Get("ui_intro_text_2"));
        }

        protected override void OnInitialize()
        {
            View.Continue += OnContinue;
        }

        protected override void OnTerminate()
        {
            View.Continue -= OnContinue;
        }

        protected override void OnViewInitialized()
        {
            View.Text = this.texts.Dequeue();
        }

        private void OnContinue()
        {
            if (this.texts.Count == 0)
            {
                Game.Instance.SwitchState(new TownGameState());
                return;
            }

            ScreenFade.Instance.To(() => { View.Text = this.texts.Dequeue(); });
        }
    }
}
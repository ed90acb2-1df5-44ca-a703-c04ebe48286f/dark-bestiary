using System.Collections.Generic;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class OutroViewController : ViewController<IIntroView>
    {
        private readonly Queue<string> texts;

        public OutroViewController(IIntroView view) : base(view)
        {
            this.texts = new Queue<string>();
            this.texts.Enqueue(I18N.Instance.Get("ui_outro_text_1"));
            this.texts.Enqueue(I18N.Instance.Get("ui_outro_text_2"));
            this.texts.Enqueue(I18N.Instance.Get("ui_outro_text_3"));
            this.texts.Enqueue(I18N.Instance.Get("ui_outro_text_4"));
            this.texts.Enqueue(I18N.Instance.Get("ui_outro_text_5"));
        }

        protected override void OnInitialize()
        {
            View.Continue += OnContinue;
            View.Text = this.texts.Dequeue();
        }

        protected override void OnTerminate()
        {
            View.Continue -= OnContinue;
        }

        private void OnContinue()
        {
            if (this.texts.Count == 0)
            {
                Game.Instance.ToCredits();
                return;
            }

            ScreenFade.Instance.To(() => { View.Text = this.texts.Dequeue(); });
        }
    }
}
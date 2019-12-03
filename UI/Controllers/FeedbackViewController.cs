using System.Collections;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;
using UnityEngine.Networking;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace DarkBestiary.UI.Controllers
{
    public class FeedbackViewController : ViewController<IFeedbackView>
    {
        public FeedbackViewController(IFeedbackView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Submitted += OnFormSubmitted;
        }

        protected override void OnTerminate()
        {
            View.Submitted -= OnFormSubmitted;
        }

        private void OnFormSubmitted(FeedbackRequest request)
        {
            var form = new WWWForm();
            form.AddField("subject", request.Subject);
            form.AddField("title", request.Title);
            form.AddField("text", request.Text);
            form.AddField("version", Game.Instance.Version);

#if !DISABLESTEAMWORKS

            if (SteamManager.Initialized)
            {
                form.AddField("steam_user_id", SteamUser.GetSteamID().ToString());
            }

#endif

            View.SetFormFieldsInteractable(false);

            Timer.Instance.StartCoroutine(SendRequest(form));
        }

        private IEnumerator SendRequest(WWWForm form)
        {
            using (var www = UnityWebRequest.Post("https://darkbestiary.com/feedback", form))
            {
                www.SetRequestHeader("Accept", "application/json");

                yield return www.SendWebRequest();

                View.SetFormFieldsInteractable(true);

                if (www.isNetworkError || www.isHttpError)
                {
                    var feedbackResponse = JsonUtility.FromJson<FeedbackResponse>(www.downloadHandler.text);
                    UiErrorFrame.Instance.Push(feedbackResponse?.message ?? www.error);
                    yield break;
                }

                ConfirmationWindow.Instance.Show("Thank you for your feedback!", I18N.Instance.Get("ui_okay"), true);
                View.ClearFormFields();
                View.Hide();
            }
        }
    }
}
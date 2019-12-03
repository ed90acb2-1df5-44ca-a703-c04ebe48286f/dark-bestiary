using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace DarkBestiary.Analytics
{
    public class CustomAnalyticsService : IAnalyticsService
    {
        private readonly string endpoint;

        public CustomAnalyticsService(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public void Event(string name, Dictionary<string, object> payload)
        {
            var data = new Dictionary<string, string>
            {
                {"Version", Game.Instance.Version},
                {"Event", name},
                {"Data", JsonConvert.SerializeObject(payload)},
            };

#if !DISABLESTEAMWORKS

            if (SteamManager.Initialized)
            {
                data.Add("SteamUserId", SteamUser.GetSteamID().ToString());
            }

#endif

            Timer.Instance.StartCoroutine(Send(data));
        }

        private IEnumerator Send(Dictionary<string, string> payload)
        {
            using (var request = UnityWebRequest.Post(this.endpoint, payload))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.url + " " + request.error);
                }
            }
        }
    }
}
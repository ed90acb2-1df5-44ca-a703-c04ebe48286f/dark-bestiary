using System.Collections;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemPickupPopup : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemStackCountText;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Construct(Relic relic)
        {
            this.itemNameText.text = relic.Name;
            this.itemStackCountText.text = "";
            this.icon.sprite = Resources.Load<Sprite>(relic.Icon);
        }

        public void Construct(Item item)
        {
            this.itemNameText.text = item.ColoredName;
            this.itemStackCountText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
            this.icon.sprite = Resources.Load<Sprite>(item.Icon);
        }

        protected override void OnSpawn()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            this.canvasGroup.alpha = 1;

            yield return new WaitForSecondsRealtime(5.0f);

            this.canvasGroup.FadeOut(1);

            yield return new WaitForSecondsRealtime(1.0f);

            Despawn();
        }
    }
}
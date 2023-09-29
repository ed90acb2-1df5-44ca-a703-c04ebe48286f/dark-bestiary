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
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private TextMeshProUGUI m_ItemStackCountText;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        public void Construct(Relic relic)
        {
            m_ItemNameText.text = relic.Name;
            m_ItemStackCountText.text = "";
            m_Icon.sprite = Resources.Load<Sprite>(relic.Icon);
        }

        public void Construct(Item item)
        {
            m_ItemNameText.text = item.ColoredName;
            m_ItemStackCountText.text = item.StackCount > 1 ? item.StackCount.ToString() : "";
            m_Icon.sprite = Resources.Load<Sprite>(item.Icon);
        }

        protected override void OnSpawn()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            m_CanvasGroup.alpha = 1;

            yield return new WaitForSecondsRealtime(5.0f);

            m_CanvasGroup.FadeOut(1);

            yield return new WaitForSecondsRealtime(1.0f);

            Despawn();
        }
    }
}
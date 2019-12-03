using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipDifference : PoolableMonoBehaviour
    {
        [SerializeField] private Image arrowUp;
        [SerializeField] private Image arrowDown;
        [SerializeField] private TextMeshProUGUI text;

        public void Construct(bool increase, string text)
        {
            this.arrowUp.gameObject.SetActive(increase);
            this.arrowDown.gameObject.SetActive(!increase);
            this.text.text = text;
        }
    }
}
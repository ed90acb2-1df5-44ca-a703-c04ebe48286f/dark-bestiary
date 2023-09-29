using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class BlockScrollOnOverflowedTooltip : MonoBehaviour
    {
        private void Start()
        {
            ItemTooltip.Instance.Shown += OnTooltipShown;
            ItemTooltip.Instance.Hidden += OnTooltipHidden;
        }

        private void OnDestroy()
        {
            ItemTooltip.Instance.Shown -= OnTooltipShown;
            ItemTooltip.Instance.Hidden -= OnTooltipHidden;

            OnTooltipHidden(null);
        }

        private void OnTooltipShown(ItemTooltip tooltip)
        {
            if (!tooltip.HasOverflow)
            {
                return;
            }

            GetComponent<ScrollRect>().enabled = false;
        }

        private void OnTooltipHidden(ItemTooltip tooltip)
        {
            GetComponent<ScrollRect>().enabled = true;
        }
    }
}
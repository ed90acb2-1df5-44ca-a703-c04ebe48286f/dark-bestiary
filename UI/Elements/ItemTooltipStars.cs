using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipStars : MonoBehaviour
    {
        [SerializeField] private Image[] stars;

        public void Construct(int count)
        {
            for (var i = 0; i < this.stars.Length; i++)
            {
                this.stars[i].gameObject.SetActive(i < count);
            }
        }
    }
}
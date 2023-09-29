using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipStars : MonoBehaviour
    {
        [SerializeField] private Image[] m_Stars;

        public void Construct(int count)
        {
            for (var i = 0; i < m_Stars.Length; i++)
            {
                m_Stars[i].gameObject.SetActive(i < count);
            }
        }
    }
}
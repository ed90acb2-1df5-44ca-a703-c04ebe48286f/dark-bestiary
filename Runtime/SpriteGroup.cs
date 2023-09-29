using System.Collections.Generic;
using Anima2D;
using UnityEngine;

namespace DarkBestiary
{
    public class SpriteGroup : MonoBehaviour
    {
        [SerializeField] private List<SpriteMeshInstance> m_Renderers;
        [SerializeField] private List<SpriteRenderer> m_SpriteRenderers;

        public void ChangeColor(Color color)
        {
            foreach (var element in m_Renderers)
            {
                element.color = color;
            }

            foreach (var element in m_SpriteRenderers)
            {
                element.color = color;
            }
        }
    }
}
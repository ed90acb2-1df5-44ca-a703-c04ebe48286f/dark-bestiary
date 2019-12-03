using System.Collections.Generic;
using Anima2D;
using UnityEngine;

namespace DarkBestiary
{
    public class SpriteGroup : MonoBehaviour
    {
        [SerializeField] private List<SpriteMeshInstance> renderers;
        [SerializeField] private List<SpriteRenderer> spriteRenderers;

        public void ChangeColor(Color color)
        {
            foreach (var element in this.renderers)
            {
                element.color = color;
            }

            foreach (var element in this.spriteRenderers)
            {
                element.color = color;
            }
        }
    }
}
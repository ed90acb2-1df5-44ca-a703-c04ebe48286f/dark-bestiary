using UnityEngine;
using UnityEngine.Rendering;

namespace DarkBestiary
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SortingGroup))]
    public class SortingGroupOrderer : MonoBehaviour
    {
        private SortingGroup m_SortingGroup;

        private void Start()
        {
            m_SortingGroup = GetComponent<SortingGroup>();
        }

        private void Update()
        {
            m_SortingGroup.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

            if (Application.isPlaying && gameObject.isStatic)
            {
                enabled = false;
            }
        }
    }
}
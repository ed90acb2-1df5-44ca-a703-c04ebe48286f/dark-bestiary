using UnityEngine;
using UnityEngine.Rendering;

namespace DarkBestiary
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SortingGroup))]
    public class SortingGroupOrderer : MonoBehaviour
    {
        private SortingGroup sortingGroup;

        private void Start()
        {
            this.sortingGroup = GetComponent<SortingGroup>();
        }

        private void Update()
        {
            this.sortingGroup.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }
    }
}
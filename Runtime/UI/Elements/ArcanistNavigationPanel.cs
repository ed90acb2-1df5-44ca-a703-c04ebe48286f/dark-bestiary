using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ArcanistNavigationPanel : MonoBehaviour
    {
        [SerializeField]
        private Tab m_TabPrefab;

        [SerializeField]
        private Transform m_TabContainer;

        private Tab m_BestiaryTab = null!;
        private Tab m_AlchemyTab = null!;
        private Tab m_TransmutationTab = null!;
        private Tab m_SphereCraftTab = null!;
    }
}
using UnityEngine;
using Screen = DarkBestiary.GameStates.Screen;

namespace DarkBestiary.UI.Elements
{
    public class VendorNavigationPanel : MonoBehaviour
    {
        [SerializeField]
        private Tab m_TabPrefab = null!;

        [SerializeField]
        private Transform m_TabContainer = null!;

        private Screen? m_GameState;
        private Tab m_Vendor = null!;
        private Tab m_Gamble = null!;
    }
}
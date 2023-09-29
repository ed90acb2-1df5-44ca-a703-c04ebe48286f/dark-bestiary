using UnityEngine;
using Screen = DarkBestiary.GameStates.Screen;

namespace DarkBestiary.UI.Elements
{
    public class BlacksmithNavigationPanel : MonoBehaviour
    {
        [SerializeField]
        private Tab m_TabPrefab = null!;

        [SerializeField]
        private Transform m_TabContainer = null!;

        private Screen? m_GameState;
        private Tab m_CraftTab = null!;
        private Tab m_RemoveGemsTab = null!;
        private Tab m_SocketingTab = null!;
        private Tab m_DismantlingTab = null!;
    }
}
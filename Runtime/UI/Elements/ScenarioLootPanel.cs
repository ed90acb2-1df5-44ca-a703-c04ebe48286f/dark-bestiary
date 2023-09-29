using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioLootPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_CurrentLevelText;
        [SerializeField] private TextMeshProUGUI m_NextLevelText;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private InventoryItem m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;
    }
}
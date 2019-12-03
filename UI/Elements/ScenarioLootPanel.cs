using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioLootPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI nextLevelText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform itemContainer;
    }
}
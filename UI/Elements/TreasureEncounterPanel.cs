using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TreasureEncounterPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void ChangeBombCount(int marked, int total)
        {
            this.text.text = $"{I18N.Instance.Get("ui_explosives")}: {marked}/{total}";
        }
    }
}
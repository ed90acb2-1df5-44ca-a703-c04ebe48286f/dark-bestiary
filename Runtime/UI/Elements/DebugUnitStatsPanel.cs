using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DebugUnitStatsPanel : MonoBehaviour
    {
        [SerializeField] private CharacterInfoRow m_TextPrefab;
        [SerializeField] private Transform m_TextContainer;

        private void Start()
        {
            SelectionManager.Instance.AllySelected += OnEntitySelected;
            SelectionManager.Instance.EnemySelected += OnEntitySelected;
        }

        private void OnEntitySelected(GameObject entity)
        {
            Cleanup();

            Instantiate(m_TextPrefab, m_TextContainer).Construct("", "");

            foreach (var attribute in entity.GetComponent<AttributesComponent>().Attributes.OrderBy(pair => pair.Value.Index))
            {
                Instantiate(m_TextPrefab, m_TextContainer).Construct(attribute.Value.Name, ((int) attribute.Value.Value()).ToString());
            }

            Instantiate(m_TextPrefab, m_TextContainer).Construct("", "");

            foreach (var property in entity.GetComponent<PropertiesComponent>().Properties.OrderBy(pair => pair.Value.Index))
            {
                Instantiate(m_TextPrefab, m_TextContainer).Construct(property.Value.Name, property.Value.ValueString());
            }
        }

        private void Cleanup()
        {
            foreach (var text in m_TextContainer.GetComponentsInChildren<CharacterInfoRow>())
            {
                Destroy(text.gameObject);
            }
        }
    }
}
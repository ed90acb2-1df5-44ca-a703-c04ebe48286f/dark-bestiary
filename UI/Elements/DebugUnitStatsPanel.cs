using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DebugUnitStatsPanel : MonoBehaviour
    {
        [SerializeField] private CharacterInfoRow textPrefab;
        [SerializeField] private Transform textContainer;

        private void Start()
        {
            SelectionManager.Instance.AllySelected += OnEntitySelected;
            SelectionManager.Instance.EnemySelected += OnEntitySelected;
        }

        private void OnEntitySelected(GameObject entity)
        {
            Cleanup();

            Instantiate(this.textPrefab, this.textContainer).Construct("", "");

            foreach (var attribute in entity.GetComponent<AttributesComponent>().Attributes.OrderBy(pair => pair.Value.Index))
            {
                Instantiate(this.textPrefab, this.textContainer).Construct(attribute.Value.Name, ((int) attribute.Value.Value()).ToString());
            }

            Instantiate(this.textPrefab, this.textContainer).Construct("", "");

            foreach (var property in entity.GetComponent<PropertiesComponent>().Properties.OrderBy(pair => pair.Value.Index))
            {
                Instantiate(this.textPrefab, this.textContainer).Construct(property.Value.Name, property.Value.ValueString());
            }
        }

        private void Cleanup()
        {
            foreach (var text in this.textContainer.GetComponentsInChildren<CharacterInfoRow>())
            {
                Destroy(text.gameObject);
            }
        }
    }
}
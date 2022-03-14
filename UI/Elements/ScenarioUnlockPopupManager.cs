using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioUnlockPopupManager : Singleton<ScenarioUnlockPopupManager>
    {
        [SerializeField] private TextPopup popupPrefab;

        private IScenarioDataRepository scenarioRepository;
        private MonoBehaviourPool<TextPopup> pool;

        private void Start()
        {
            this.scenarioRepository = Container.Instance.Resolve<IScenarioDataRepository>();

            this.pool = MonoBehaviourPool<TextPopup>.Factory(
                this.popupPrefab, UIManager.Instance.PopupContainer, 2);

            Character.ScenarioUnlocked += OnScenarioUnlocked;
        }

        private void OnScenarioUnlocked(int scenarioId)
        {
            var nameKey = this.scenarioRepository.Find(scenarioId).NameKey;
            this.pool.Spawn().Construct(I18N.Instance.Translate(nameKey));
        }
    }
}
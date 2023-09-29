using System;
using System.Collections;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios.Scenes;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    public class CreditsScene : MonoBehaviour
    {
        public event Action Skipped;

        [SerializeField] private Interactable m_ContinueButton;

        private ISceneRepository m_SceneRepository;
        private IUnitRepository m_UnitRepository;
        private Scene m_Scene;

        public void Initialize()
        {
            m_SceneRepository = Container.Instance.Resolve<ISceneRepository>();
            m_UnitRepository = Container.Instance.Resolve<IUnitRepository>();

            m_ContinueButton.PointerClick += OnContinueButtonPointerClick;

            NextScene();
        }

        public void Terminate()
        {
            m_ContinueButton.PointerClick -= OnContinueButtonPointerClick;
            m_Scene.Terminate();
            Destroy(gameObject);
        }

        private void OnContinueButtonPointerClick()
        {
            Skipped?.Invoke();
        }

        private void NextScene()
        {
            if (m_Scene != null)
            {
                m_Scene.Terminate();

                foreach (var entity in m_Scene.Entities.All())
                {
                    entity.Terminate();
                }
            }

            m_Scene = RandomScene();
            m_Scene.Initialize();

            Wait(1, () =>
            {
                Pathfinder.Instance.Scan();

                Wait(9, () => { ScreenFade.Instance.To(NextScene); });
            });
        }

        private void OnExitDoor(GameObject entity)
        {
            entity.GetComponent<ActorComponent>().Model.FadeOut(1.0f);
        }

        private Scene RandomScene()
        {
            return m_SceneRepository.Random(s => !s.HasNoExit && !s.IsScripted);
        }

        private GameObject RandomUnit()
        {
            var unit = m_UnitRepository.Random(u =>
                !u.Flags.HasFlag(UnitFlags.Dummy) &&
                !u.Flags.HasFlag(UnitFlags.Playable) &&
                !u.Flags.HasFlag(UnitFlags.Immovable));

            var unitComponent = unit.GetComponent<UnitComponent>();
            unitComponent.Owner = Owner.Player;
            unitComponent.TeamId = 1;

            return unit;
        }

        private void Wait(float seconds, Action action)
        {
            StartCoroutine(WaitCoroutine(seconds, action));
        }

        private IEnumerator WaitCoroutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }
    }
}
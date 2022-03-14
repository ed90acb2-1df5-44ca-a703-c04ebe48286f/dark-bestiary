using System;
using System.Collections;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios.Scenes;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    public class CreditsScene : MonoBehaviour
    {
        public event Payload Skipped;

        [SerializeField] private Interactable continueButton;

        private ISceneRepository sceneRepository;
        private IUnitRepository unitRepository;
        private Scene scene;

        public void Initialize()
        {
            this.sceneRepository = Container.Instance.Resolve<ISceneRepository>();
            this.unitRepository = Container.Instance.Resolve<IUnitRepository>();

            this.continueButton.PointerClick += OnContinueButtonPointerClick;

            NextScene();
        }

        public void Terminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
            this.scene.ExitDoor -= OnExitDoor;
            this.scene.Terminate();
            Destroy(gameObject);
        }

        private void OnContinueButtonPointerClick()
        {
            Skipped?.Invoke();
        }

        private void NextScene()
        {
            if (this.scene != null)
            {
                this.scene.ExitDoor -= OnExitDoor;
                this.scene.Terminate();

                foreach (var entity in this.scene.Entities.All())
                {
                    entity.Terminate();
                }
            }

            this.scene = RandomScene();
            this.scene.ExitDoor += OnExitDoor;
            this.scene.Initialize();
            this.scene.OpenExitDoor();

            Wait(1, () =>
            {
                Pathfinder.Instance.Scan();

                if (this.scene.EnterLocation != Vector3.zero && this.scene.ExitLocation != Vector3.zero)
                {
                    CreateAndMoveUnits();
                }

                Wait(9, () =>
                {
                    ScreenFade.Instance.To(NextScene);
                });
            });
        }

        private void CreateAndMoveUnits()
        {
            for (var i = 0; i < RNG.Range(1, 3); i++)
            {
                Wait(i, () =>
                {
                    var unit = RandomUnit();
                    unit.transform.position = BoardNavigator.Instance.NearestCell(this.scene.EnterLocation).transform.position;
                    unit.GetComponent<MovementComponent>().Move(this.scene.ExitLocation);
                    unit.GetComponent<UnitComponent>().Flags |= UnitFlags.Dummy;
                });
            }
        }

        private void OnExitDoor(GameObject entity)
        {
            entity.GetComponent<ActorComponent>().Model.FadeOut(1.0f);
        }

        private Scene RandomScene()
        {
            return this.sceneRepository.Random(s => !s.HasNoExit && !s.IsScripted);
        }

        private GameObject RandomUnit()
        {
            var unit = this.unitRepository.Random(u =>
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
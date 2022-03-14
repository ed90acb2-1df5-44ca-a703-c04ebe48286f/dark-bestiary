using System;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    [Serializable]
    public struct AltarEffectInfo
    {
        public int EffectId;
        public GameObject Prefab;
        public string NameKey;
        public string DescriptionKey;
    }

    public class Altar : InteractableObject
    {
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite hover;
        [SerializeField] private SpriteRenderer graphics;
        [SerializeField] private Transform container;
        [SerializeField] private AltarEffectInfo[] effects;

        private Effect effect;
        private CharacterManager characterManager;
        private Interactor interactor;
        private GameObject representation;

        private void Start()
        {
            var info = this.effects.Random();

            this.effect = Container.Instance.Resolve<IEffectRepository>().Find(info.EffectId);
            this.representation = Instantiate(info.Prefab, this.container);
            this.characterManager = Container.Instance.Resolve<CharacterManager>();
            this.interactor = Container.Instance.Resolve<Interactor>();

            Construct(I18N.Instance.Get(info.NameKey), I18N.Instance.Get(info.DescriptionKey));

            OnPointerExit();
        }

        protected override void OnPointerEnter()
        {
            this.graphics.sprite = this.hover;

            if (this.interactor.State is CastState || this.interactor.State is MoveState)
            {
                return;
            }

            BoardNavigator.Instance.HighlightRadius(transform.position, 2, Color.white.With(a: 0.15f));
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Interact);
        }

        protected override void OnPointerExit()
        {
            this.graphics.sprite = this.normal;

            if (this.interactor.State is CastState || this.interactor.State is MoveState)
            {
                return;
            }

            BoardNavigator.Instance.Board.Clear();
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
        }

        protected override void OnPointerUp()
        {
            var acting = this.characterManager.Character.Entity;

            if (CombatEncounter.Active?.Acting.IsOwnedByPlayer() == true)
            {
                acting = CombatEncounter.Active.Acting;
            }

            if ((acting.transform.position - transform.position).magnitude >= 2.5f)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_target_is_too_far"));
                return;
            }

            if (CombatEncounter.Active != null && !CombatEncounter.Active.IsEntityTurn(acting))
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_wait_your_turn"));
                return;
            }

            Consume(acting);
        }

        private void Consume(GameObject entity)
        {
            if (this.representation == null)
            {
                return;
            }

            this.effect.Apply(entity, transform.position);

            this.representation.DestroyAsVisualEffect();
            this.representation = null;
        }
    }
}
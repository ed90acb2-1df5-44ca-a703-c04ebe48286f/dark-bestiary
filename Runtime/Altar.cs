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
        [SerializeField]
        private Sprite m_Normal = null!;

        [SerializeField]
        private Sprite m_Hover = null!;

        [SerializeField]
        private SpriteRenderer m_Graphics = null!;

        [SerializeField]
        private Transform m_Container = null!;

        [SerializeField]
        private AltarEffectInfo[] m_Effects = null!;

        private Effect m_Effect;
        private Interactor m_Interactor;
        private GameObject m_Representation;

        private void Start()
        {
            var info = m_Effects.Random();

            m_Effect = Container.Instance.Resolve<IEffectRepository>().Find(info.EffectId);
            m_Representation = Instantiate(info.Prefab, m_Container);
            m_Interactor = Container.Instance.Resolve<Interactor>();

            Construct(I18N.Instance.Get(info.NameKey), I18N.Instance.Get(info.DescriptionKey));

            OnPointerExit();
        }

        protected override void OnPointerEnter()
        {
            m_Graphics.sprite = m_Hover;

            if (m_Interactor.State is CastState || m_Interactor.State is MoveState)
            {
                return;
            }

            BoardNavigator.Instance.HighlightRadius(transform.position, 2, Color.white.With(a: 0.15f));
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Interact);
        }

        protected override void OnPointerExit()
        {
            m_Graphics.sprite = m_Normal;

            if (m_Interactor.State is CastState || m_Interactor.State is MoveState)
            {
                return;
            }

            BoardNavigator.Instance.Board.Clear();
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
        }

        protected override void OnPointerUp()
        {
            var acting = Game.Instance.Character.Entity;

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
            if (m_Representation == null)
            {
                return;
            }

            m_Effect.Apply(entity, transform.position);

            m_Representation.DestroyAsVisualEffect();
            m_Representation = null;
        }
    }
}
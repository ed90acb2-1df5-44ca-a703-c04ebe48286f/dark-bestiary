using System;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CursorManager : Singleton<CursorManager>
    {
        public enum CursorState
        {
            None,
            Normal,
            Cast,
            Loot,
            Move,
            Interact,
        }

        [SerializeField] private Texture2D m_Normal;
        [SerializeField] private Texture2D m_Cast;
        [SerializeField] private Texture2D m_Loot;
        [SerializeField] private Texture2D m_Move;
        [SerializeField] private Texture2D m_Interact;

        private CursorState m_CurrentState = CursorState.None;
        private CursorState m_PreviousState = CursorState.None;

        private void Start()
        {
            Episode.AnyEpisodeTerminated += OnAnyEpisodeTerminated;
            Scenario.AnyScenarioExit += OnAnyScenarioExit;

            ChangeState(CursorState.Normal);
        }

        private void OnAnyScenarioExit(Scenario scenario)
        {
            ChangeState(CursorState.Normal);
        }

        public void SetPreviousState()
        {
            ChangeState(m_PreviousState);
        }

        public void ChangeState(CursorState state)
        {
            if (m_CurrentState == state)
            {
                return;
            }

            Cursor.visible = state != CursorState.None;

            switch (state)
            {
                case CursorState.None:
                    Cursor.SetCursor(m_Normal, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Normal:
                    Cursor.SetCursor(m_Normal, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Loot:
                    Cursor.SetCursor(m_Loot, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Interact:
                    Cursor.SetCursor(m_Interact, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Cast:
                    var hotspot = new Vector2((float) m_Cast.width / 2, (float) m_Cast.height / 2);
                    Cursor.SetCursor(m_Cast, hotspot, CursorMode.ForceSoftware);
                    break;
                case CursorState.Move:
                    Cursor.SetCursor(m_Move, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            m_PreviousState = m_CurrentState == CursorState.None ? CursorState.Normal : m_CurrentState;
            m_CurrentState = state;
        }

        private void OnAnyEpisodeTerminated(Episode payload)
        {
            ChangeState(CursorState.Normal);
        }
    }
}
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

        [SerializeField] private Texture2D normal;
        [SerializeField] private Texture2D cast;
        [SerializeField] private Texture2D loot;
        [SerializeField] private Texture2D move;
        [SerializeField] private Texture2D interact;

        private CursorState currentState = CursorState.None;
        private CursorState previousState = CursorState.None;

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
            ChangeState(this.previousState);
        }

        public void ChangeState(CursorState state)
        {
            if (this.currentState == state)
            {
                return;
            }

            Cursor.visible = state != CursorState.None;

            switch (state)
            {
                case CursorState.None:
                    Cursor.SetCursor(this.normal, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Normal:
                    Cursor.SetCursor(this.normal, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Loot:
                    Cursor.SetCursor(this.loot, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Interact:
                    Cursor.SetCursor(this.interact, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case CursorState.Cast:
                    var hotspot = new Vector2((float) this.cast.width / 2, (float) this.cast.height / 2);
                    Cursor.SetCursor(this.cast, hotspot, CursorMode.ForceSoftware);
                    break;
                case CursorState.Move:
                    Cursor.SetCursor(this.move, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            this.previousState = this.currentState == CursorState.None ? CursorState.Normal : this.currentState;
            this.currentState = state;
        }

        private void OnAnyEpisodeTerminated(Episode payload)
        {
            ChangeState(CursorState.Normal);
        }
    }
}
using System;
using System.Linq;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Scenarios.Scenes
{
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        public event Action<GameObject> Entered;

        [SerializeField] private bool m_DisableCell;
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Sprite m_Opened;
        [SerializeField] private Sprite m_Closed;

        private BoardCell m_Cell;
        private bool m_IsOpened;

        public void Open()
        {
            m_IsOpened = true;
            m_SpriteRenderer.sprite = m_Opened;

            if (m_Animator != null)
            {
                m_Animator.Play("open");
            }

            if (m_DisableCell)
            {
                EnableCell();
            }

            GetComponent<Collider2D>().enabled = true;
        }

        public void Close()
        {
            m_IsOpened = false;
            m_SpriteRenderer.sprite = m_Closed;

            if (m_DisableCell)
            {
                DisableCell();
            }

            GetComponent<Collider2D>().enabled = false;
        }

        public void EnableCell()
        {
            if (!Board.Instance.gameObject.activeSelf)
            {
                return;
            }

            GetCell().gameObject.SetActive(true);
        }

        public void DisableCell()
        {
            if (!Board.Instance.gameObject.activeSelf)
            {
                return;
            }

            GetCell().gameObject.SetActive(false);
        }

        private BoardCell GetCell()
        {
            if (m_Cell == null)
            {
                m_Cell = BoardNavigator.Instance
                    .WithinCircle(transform.position, 1)
                    .OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
                    .First();
            }

            return m_Cell;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!m_IsOpened)
            {
                return;
            }

            Entered?.Invoke(collider.gameObject);
        }
    }
}
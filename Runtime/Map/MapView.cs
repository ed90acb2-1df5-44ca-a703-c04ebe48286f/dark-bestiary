using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views.Unity;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DarkBestiary.Map
{
    public class MapView : View, IMapView
    {
        public event Action<MapEncounterView>? AnyEncounterViewClicked;

        public MapEncounterView[] EncounterViews { get; private set; } = null!;
        public MapEncounterView? LastCompletedEncounterView { get; private set; }

        [Header("Config")]
        [SerializeField]
        private float m_Margin;

        [Header("Prefabs")]
        [SerializeField]
        private MapEncounterView m_MapEncounterViewPrefab = null!;

        [SerializeField]
        private Transform m_Container = null!;

        private int m_Width;
        private int m_Height;
        private Camera m_MainCamera = null!;

        private bool m_IsLocked;
        private float m_Scale = 1;
        private bool m_IsDragging;
        private Vector3 m_PreviousMousePosition;
        private GameObject m_Character = null!;

        public void Construct(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_MainCamera = Camera.main!;
            m_Character = Game.Instance.Character.Entity;
            EncounterViews = new MapEncounterView[m_Width * m_Height];
        }

        public void CreateEncounters(List<MapEncounterData> encounters)
        {
            m_Container.position = Vector3.zero;
            m_Container.localScale = Vector3.one;
            m_Scale = 1;

            var offset = new Vector3(m_Width, m_Height) * (m_Margin / 2);

            for (var index = 0; index < m_Width * m_Height; index++)
            {
                var x = index % m_Width;
                var y = index / m_Width;

                if (EncounterViews[index])
                {
                    EncounterViews[index].Clicked -= OnEncounterViewClicked;
                    EncounterViews[index].Terminate();
                }

                EncounterViews[index] = Instantiate(m_MapEncounterViewPrefab, m_Container);
                EncounterViews[index].Clicked += OnEncounterViewClicked;
                EncounterViews[index].Initialize(index, encounters[index]);
                EncounterViews[index].Lock();
                EncounterViews[index].transform.position = new Vector3(
                    x * m_Margin - offset.x + m_Margin / 2,
                    y * m_Margin - offset.y + m_Margin / 2
                );
            }
        }

        protected override void OnTerminate()
        {
            foreach (var encounterView in EncounterViews)
            {
                encounterView.Terminate();
            }
        }

        public void RunInitialRoutine()
        {
            m_IsLocked = true;

            m_Character.transform.position = new Vector3(-100, 0, 0);

            var last = EncounterViews.Last();
            last.Reveal();
            m_Container.position -= last.transform.localPosition;

            Timer.Instance.Wait(3, () =>
                {
                    var first = EncounterViews.First();

                    ScrollTo(first, () =>
                    {
                        m_Character.transform.position = first.transform.position;
                        OnEncounterCompleted(first);
                    });
                }
            );
        }

        public void OnEncounterCompleted(MapEncounterView encounterView)
        {
            LastCompletedEncounterView = encounterView;
            m_Character.transform.position = encounterView.transform.position;
            encounterView.Complete();
            HideSkipped(encounterView);
            UnlockNeighbours(encounterView);
            ShakeHiddenNeighbours();
        }

        public void ScrollTo(MapEncounterView destination, Action? callback = null)
        {
            m_IsLocked = true;

            m_Container.DOMove(m_Container.position - destination.transform.position, 30.0f)
                .SetAs(new TweenParams().SetSpeedBased().SetEase(Ease.OutExpo))
                .OnComplete(() =>
                {
                    m_IsLocked = false;
                    callback?.Invoke();
                });
        }

        public void Enter()
        {
            MapEncounterView.IsAnyActiveHovered = false;

            m_Character.transform.SetParent(m_Container);
            m_Character.transform.localScale = Vector3.one;
            m_Character.transform.position = LastCompletedEncounterView == null ? new Vector3(-100, 0, 0) : LastCompletedEncounterView.transform.position;
        }

        public void Exit()
        {
            m_Character.transform.SetParent(null);
            m_Character.transform.localScale = Vector3.one;
            m_Character.transform.position = new Vector3(-100, 0, 0);
        }

        private void OnEncounterViewClicked(MapEncounterView mapEncounterView)
        {
            if (mapEncounterView.IsCompleted || m_IsLocked)
            {
                return;
            }

            AnyEncounterViewClicked?.Invoke(mapEncounterView);
        }

        private void UnlockNeighbours(MapEncounterView mapEncounterView)
        {
            var neighbours = FindNeighbours(mapEncounterView);

            var index = 0;

            foreach (var neighbour in neighbours.OrderBy(_ => Random.value))
            {
                if (neighbour == null || neighbour.IsCompleted || neighbour.IsUnlocked || neighbour.IsHidden)
                {
                    continue;
                }

                index++;

                Timer.Instance.Wait(0.2f * index, () =>
                {
                    neighbour.Unlock();

                    foreach (var reveal in FindNeighbours(neighbour))
                    {
                        if (reveal == null || reveal.IsLocked == false || reveal.IsHidden)
                        {
                            continue;
                        }

                        reveal.Reveal();
                    }
                });
            }
        }

        private void ShakeHiddenNeighbours()
        {
            At(0, 0)?.Shake();
            At(1, 0)?.Shake();
            At(0, 1)?.Shake();

            for (var y = 0; y < m_Height; y++)
            {
                for (var x = 0; x < m_Width; x++)
                {
                    var index = x + y * m_Width;
                    var encounterView = EncounterViews[index];

                    if (encounterView.IsHidden)
                    {
                        continue;
                    }

                    if (AtOffset(encounterView, new Vector2(-1, 0))?.IsHidden == true ||
                        AtOffset(encounterView, new Vector2(0, -1))?.IsHidden == true ||
                        AtOffset(encounterView, new Vector2(0, 1))?.IsHidden == true)
                    {
                        encounterView.Shake();
                    }
                }
            }
        }

        private void HideSkipped(MapEncounterView except)
        {
            var completed = EncounterViews.Count(x => x.State == MapEncounterState.Completed);

            for (var y = 0; y < completed; y++)
            {
                for (var x = 0; x < completed - y; x++)
                {
                    var index = x + y * m_Width;

                    if (EncounterViews.IndexInBounds(index) == false || EncounterViews[index] == except || EncounterViews[index].IsHidden)
                    {
                        continue;
                    }

                    EncounterViews[index].Hide();
                }
            }
        }

        private MapEncounterView?[] FindNeighbours(MapEncounterView mapEncounterView)
        {
            return new[]
            {
                AtOffset(mapEncounterView, new Vector2(0, 1)), // TOP
                AtOffset(mapEncounterView, new Vector2(1, 1)), // TOP RIGHT
                AtOffset(mapEncounterView, new Vector2(1, 0)), // RIGHT
                AtOffset(mapEncounterView, new Vector2(1, -1)), // BOTTOM RIGHT
                AtOffset(mapEncounterView, new Vector2(0, -1)), // BOTTOM
                AtOffset(mapEncounterView, new Vector2(-1, -1)), // BOTTOM LEFT
                AtOffset(mapEncounterView, new Vector2(-1, 0)), // LEFT
                AtOffset(mapEncounterView, new Vector2(-1, 1)), // TOP LEFT
            };
        }

        public List<MapEncounterView> WithinRange(Vector3 origin, int range)
        {
            var radius = m_Margin * m_Scale * range;
            var colliders = new Collider2D[256];
            var count = Physics2D.OverlapCircleNonAlloc(origin, radius, colliders);

            return colliders.Take(count).Select(c => c.gameObject.GetComponent<MapEncounterView>()).NotNull().ToList();
        }

        private MapEncounterView? AtOffset(MapEncounterView mapEncounterView, Vector2 offset)
        {
            var x = mapEncounterView.Index % m_Width + (int) offset.x;
            var y = mapEncounterView.Index / m_Width + (int) offset.y;

            if (x < 0 || x > m_Width - 1 ||
                y < 0 || y > m_Height - 1)
            {
                return null;
            }

            var index = x + y * m_Width;

            return EncounterViews[index];
        }

        private MapEncounterView? At(int x, int y)
        {
            var index = x + y * m_Width;

            if (EncounterViews.IndexInBounds(index) == false)
            {
                return null;
            }

            return EncounterViews[index];
        }

        public void ResetScale()
        {
            m_Scale = 1;
            m_Container.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
        }

        private void Update()
        {
            var isBlockedByUi = UIManager.Instance.IsGameFieldBlockedByUI();

            if (Input.GetMouseButtonDown(0) && !isBlockedByUi && !MapEncounterView.IsAnyActiveHovered)
            {
                m_IsDragging = true;
            }

            if (Input.GetMouseButtonUp(0) && m_IsDragging)
            {
                m_IsDragging = false;
            }

            var isZooming = Mathf.Abs(Input.mouseScrollDelta.y) > 0 && !isBlockedByUi;

            if (isZooming)
            {
                m_Scale = Mathf.Clamp(m_Scale + 0.1f * Mathf.Sign(Input.mouseScrollDelta.y), 0.5f, 1.5f);
                m_Container.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
            }

            if ((m_IsDragging || isZooming) && !m_IsLocked)
            {
                var positionA = m_MainCamera.ScreenToWorldPoint(m_PreviousMousePosition);
                var positionB = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                var position = m_Container.position + (positionB - positionA).normalized * (positionA - positionB).magnitude;
                var rectWidth = (m_Width - 1) * m_Margin * m_Scale;
                var rectHeight = (m_Height - 1) * m_Margin * m_Scale;

                m_Container.position = position.With(
                    Mathf.Clamp(position.x, -rectWidth / 2, rectWidth / 2),
                    Mathf.Clamp(position.y, -rectHeight / 2 - 10, rectHeight / 2));
            }

            m_PreviousMousePosition = Input.mousePosition;
        }
    }
}
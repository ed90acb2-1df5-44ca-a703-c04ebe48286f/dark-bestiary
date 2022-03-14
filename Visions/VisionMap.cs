using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Randomization;
using DG.Tweening;
using UnityEngine;

namespace DarkBestiary.Visions
{
    public class VisionMap : MonoBehaviour
    {
        public event Payload<VisionView> AnyVisionClicked;
        public event Payload Completed;

        public Transform VisionContainer => this.visionContainer;
        public bool IsLocked { get; set; }
        public VisionView[] VisionViews { get; private set; }
        public VisionView FinalVision { get; private set; }

        [Header("Config")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float margin;

        [Header("Prefabs")]
        [SerializeField] private VisionView finalVisionPrefab;
        [SerializeField] private VisionView visionPrefab;
        [SerializeField] private Transform visionContainer;

        private readonly List<int> uniqueVisions = new List<int>();

        private float scale = 1;
        private VisionView[] neighbours = Array.Empty<VisionView>();
        private Vector3 previousMousePosition;
        private Camera mainCamera;
        private IVisionDataRepository visionRepository;
        private bool isDragging;

        public void Initialize()
        {
            this.visionRepository = Container.Instance.Resolve<IVisionDataRepository>();
            this.mainCamera = Camera.main;

            VisionViews = new VisionView[this.width * this.height];

            VisionView.AnyClicked += OnAnyVisionClicked;
        }

        public void Terminate()
        {
            VisionView.AnyClicked -= OnAnyVisionClicked;

            foreach (var vision in VisionViews)
            {
                vision?.Terminate();
            }

            FinalVision?.Terminate();

            Destroy(gameObject);
        }

        public void Generate(int actIndex)
        {
            CreateVisionViews(actIndex, CreateVisions(actIndex));
            RunInitialRoutine(actIndex);
        }

        public void CreateVisionViews(int actIndex, List<VisionData> visions)
        {
            this.visionContainer.position = Vector3.zero;
            this.visionContainer.localScale = Vector3.one;
            this.scale = 1;

            var offset = new Vector3(this.width, this.height) * (this.margin / 2);

            for (var index = 0; index < this.width * this.height; index++)
            {
                var x = index % this.width;
                var y = index / this.width;

                if (VisionViews[index])
                {
                    VisionViews[index].Terminate();
                }

                VisionViews[index] = Instantiate(this.visionPrefab, this.visionContainer);
                VisionViews[index].Initialize(index, actIndex, visions[index]);
                VisionViews[index].Lock();
                VisionViews[index].transform.position = new Vector3(
                    x * this.margin - offset.x + this.margin / 2,
                    y * this.margin - offset.y + this.margin / 2
                );
            }
        }

        private void RunInitialRoutine(int actIndex)
        {
            CreateFinalVision(actIndex);
            FinalVision.gameObject.SetActive(false);

            IsLocked = true;

            Timer.Instance.Wait(0.5f, () =>
                {
                    FinalVision.gameObject.SetActive(true);
                    FinalVision.Unlock();
                }
            );

            Timer.Instance.Wait(3, () =>
                {
                    var first = GetFirstVision();

                    ScrollTo(first.transform, () =>
                    {
                        first.Unlock();

                        if (actIndex == 1)
                        {
                            OnAnyVisionClicked(first);
                        }
                    });
                }
            );
        }

        public void SetCurrentVision(VisionView visionView)
        {
            this.neighbours = GetNeighbours(visionView);

            visionView.SetArrowsEnabled(
                !this.neighbours[0]?.IsCompletedOrSkipped == true,
                !this.neighbours[1]?.IsCompletedOrSkipped == true,
                !this.neighbours[2]?.IsCompletedOrSkipped == true,
                !this.neighbours[3]?.IsCompletedOrSkipped == true
            );
        }

        public void OnVisionCompleted(VisionView vision)
        {
            vision.Complete();

            foreach (var neighbour in this.neighbours)
            {
                if (neighbour == null || neighbour.IsCompleted)
                {
                    continue;
                }

                neighbour.Skip();
            }

            if (vision.Data.IsFinal)
            {
                Completed?.Invoke();
                return;
            }

            this.neighbours = GetNeighbours(vision);

            if (this.neighbours.All(neighbour => neighbour == null || neighbour.IsSkipped || neighbour.IsCompleted))
            {
                ScrollTo(FinalVision.transform);
                return;
            }

            UnlockNeighbours();

            vision.SetArrowsEnabled(
                !this.neighbours[0]?.IsCompletedOrSkipped == true,
                !this.neighbours[1]?.IsCompletedOrSkipped == true,
                !this.neighbours[2]?.IsCompletedOrSkipped == true,
                !this.neighbours[3]?.IsCompletedOrSkipped == true
            );
        }

        private List<VisionData> CreateVisions(int actIndex)
        {
            var count = this.width * this.height;
            var table = new RandomizerTable(count);

            bool Predicate(VisionData data)
            {
                return data.IsEnabled && actIndex.InRange(data.ActMin, data.ActMax);
            }

            foreach (var vision in this.visionRepository.Find(Predicate).Select(PrepareVisionData))
            {
                table.Add(new RandomizerVisionDataValue(
                    vision,
                    vision.Probability,
                    vision.IsUnique,
                    vision.IsGuaranteed,
                    vision.IsEnabled
                ));
            }

            var visions = table.Evaluate().OfType<RandomizerVisionDataValue>().Select(value => value.Value).ToList();

            if (visions.Count < count)
            {
                visions.AddRange(this.visionRepository.Find(Predicate).Random(count - visions.Count).Select(PrepareVisionData));
            }

            return visions.Shuffle().ToList();
        }

        private VisionData PrepareVisionData(VisionData visionData)
        {
            if (visionData.Type != VisionType.Random)
            {
                return visionData;
            }

            var vision = new VisionData(this.visionRepository.Find(visionData.Visions.Random()));
            vision.Icon = visionData.Icon;
            vision.Sanity = visionData.Sanity;
            vision.NameKey = visionData.NameKey;
            vision.DescriptionKey = visionData.DescriptionKey;
            vision.Probability = visionData.Probability;
            vision.IsFinal = visionData.IsFinal;
            vision.IsUnique = visionData.IsUnique;
            vision.IsGuaranteed = visionData.IsGuaranteed;
            vision.IsEnabled = visionData.IsEnabled;
            vision.RarityId = visionData.RarityId;
            vision.Sound = string.IsNullOrEmpty(visionData.Sound) ? vision.Sound : visionData.Sound;

            return PrepareVisionData(vision);
        }

        private void CreateFinalVision(int actIndex)
        {
            var finalVisionData = PrepareVisionData(this.visionRepository.Find(
                vision => actIndex.InRange(vision.ActMin, vision.ActMax) && vision.IsFinal && !this.uniqueVisions.Contains(vision.Id)).First());

            CreateFinalVision(finalVisionData, actIndex);
        }

        public void CreateFinalVision(VisionData data, int actIndex)
        {
            var finalVisionPosition = new Vector3(0, (this.height + 2) / 2f * this.margin);

            if (FinalVision != null)
            {
                FinalVision.Terminate();
            }

            FinalVision = Instantiate(this.finalVisionPrefab, finalVisionPosition, Quaternion.identity, this.visionContainer);
            FinalVision.Initialize(-1, actIndex, data);
            FinalVision.Lock();

            this.visionContainer.position -= FinalVision.transform.localPosition;
            this.uniqueVisions.Add(data.Id);
        }

        private VisionView GetFirstVision()
        {
            var highestProbability = VisionViews.Max(v => v.Data.Probability);

            return VisionViews.Where(vision => vision.Data.Type == VisionType.Scenario && Mathf.Approximately(vision.Data.Probability, highestProbability))
                .Shuffle()
                .First();
        }

        public void ScrollTo(Transform destination, Action callback = null)
        {
            IsLocked = true;

            this.visionContainer.DOMove(this.visionContainer.position - destination.position, 30.0f)
                .SetAs(new TweenParams().SetSpeedBased().SetEase(Ease.OutExpo))
                .OnComplete(() =>
                {
                    IsLocked = false;
                    callback?.Invoke();
                });
        }

        private void OnAnyVisionClicked(VisionView vision)
        {
            if (vision.IsCompleted || IsLocked)
            {
                return;
            }

            AnyVisionClicked?.Invoke(vision);
        }

        private void UnlockNeighbours()
        {
            var index = 0;

            foreach (var neighbour in this.neighbours)
            {
                neighbour?.SetArrowsEnabled(false);

                if (neighbour == null || neighbour.IsCompleted || neighbour.IsSkipped || neighbour.Data.IsFinal)
                {
                    continue;
                }

                Timer.Instance.Wait(0.33f * index, () => neighbour.Unlock());

                index++;
            }
        }

        private VisionView[] GetNeighbours(VisionView encounter)
        {
            return new[]
            {
                AtOffset(encounter, Vector2.up),
                AtOffset(encounter, Vector2.right),
                AtOffset(encounter, Vector2.down),
                AtOffset(encounter, Vector2.left)
            };
        }

        public List<VisionView> WithinRange(Vector3 origin, int range)
        {
            var radius = this.margin * this.scale * range;
            var colliders = new Collider2D[256];
            var count = Physics2D.OverlapCircleNonAlloc(origin, radius, colliders);

            return colliders.Take(count).Select(c => c.gameObject.GetComponent<VisionView>()).NotNull().ToList();
        }

        private VisionView AtOffset(VisionView vision, Vector2 offset)
        {
            var x = vision.Index % this.width + (int) offset.x;
            var y = vision.Index / this.width + (int) offset.y;

            if (x < 0 || x > this.width - 1 || y < 0 || y > this.height - 1)
            {
                return null;
            }

            var index = x + y * this.width;

            return VisionViews[index];
        }

        private void Update()
        {
            var isBlockedByUi = UIManager.Instance.IsGameFieldBlockedByUI();

            if (Input.GetMouseButtonDown(0) && !isBlockedByUi && !VisionView.IsAnyActiveHovered)
            {
                this.isDragging = true;
            }

            if (Input.GetMouseButtonUp(0) && this.isDragging)
            {
                this.isDragging = false;
            }

            var isScrolling = Mathf.Abs(Input.mouseScrollDelta.y) > 0 && !isBlockedByUi;

            if (isScrolling)
            {
                this.scale = Mathf.Clamp(this.scale + 0.1f * Mathf.Sign(Input.mouseScrollDelta.y), 0.5f, 1.5f);
                this.visionContainer.localScale = new Vector3(this.scale, this.scale, this.scale);
            }

            if ((this.isDragging || isScrolling) && !IsLocked)
            {
                var positionA = this.mainCamera.ScreenToWorldPoint(this.previousMousePosition);
                var positionB = this.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var position = this.visionContainer.position + (positionB - positionA).normalized * (positionA - positionB).magnitude;
                var rectWidth = (this.width - 1) * this.margin * this.scale;
                var rectHeight = (this.height - 1) * this.margin * this.scale;

                this.visionContainer.position = position.With(
                    Mathf.Clamp(position.x, -rectWidth / 2, rectWidth / 2),
                    Mathf.Clamp(position.y, -rectHeight / 2 - 10, rectHeight / 2));
            }

            this.previousMousePosition = Input.mousePosition;
        }
    }
}
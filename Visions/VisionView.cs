using System;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.Visions
{
    public class VisionView : MonoBehaviour
    {
        public static event Payload<VisionView> AnyClicked;

        public static bool IsAnyActiveHovered;

        public int Index { get; private set; }
        public int ActIndex { get; private set; }
        public VisionData Data { get; private set; }
        public VisionViewState State { get; private set; } = VisionViewState.Unlocked;
        public bool IsCompleted => State == VisionViewState.Completed;
        public bool IsLocked => State == VisionViewState.Locked;
        public bool IsRevealed => State == VisionViewState.Revealed;
        public bool IsUnlocked => State == VisionViewState.Unlocked;
        public bool IsSkipped => State == VisionViewState.Skipped;
        public bool IsCompletedOrSkipped => IsCompleted || IsSkipped;

        [Header("Components")]
        [SerializeField] private SpriteRenderer frameRenderer;
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform container;
        [SerializeField] private Transform tooltipAnchor;
        [SerializeField] private TextMeshProUGUI labelText;

        [Header("Colors")]
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color pressedColor;
        [SerializeField] private Color disabledColor;

        [Header("Particles")]
        [SerializeField] private GameObject magicRarityParticles;
        [SerializeField] private GameObject rareRarityParticles;
        [SerializeField] private GameObject uniqueRarityParticles;
        [SerializeField] private GameObject legendaryRarityParticles;
        [SerializeField] private GameObject mythicRarityParticles;

        [Header("Arrows")]
        [SerializeField] private SpriteRenderer arrowTop;
        [SerializeField] private SpriteRenderer arrowRight;
        [SerializeField] private SpriteRenderer arrowBottom;
        [SerializeField] private SpriteRenderer arrowLeft;

        private GameObject avatar;
        private ActorComponent avatarActor;
        private GameObject rarityParticles;

        public void Initialize(int index, int actIndex, VisionData data)
        {
            Index = index;
            ActIndex = actIndex;
            name = $"Encounter #{data.Id} ({data.Type})";
            Data = data;

            if (data.UnitId > 0)
            {
                this.avatar = Container.Instance.Resolve<IUnitRepository>().Find(data.UnitId);
                this.avatar.transform.parent = this.container;
                this.avatar.transform.localPosition = Vector3.zero;

                this.avatarActor = this.avatar.GetComponent<ActorComponent>();
                this.avatarActor.Model.FadeInOnStart = false;

                UpdateAvatarUnitLevel();
            }
            else if (!string.IsNullOrEmpty(data.Prefab))
            {
                this.avatar = Instantiate(Resources.Load<GameObject>(Data.Prefab), this.container);
            }

            this.iconRenderer.enabled = false;
            this.iconRenderer.sprite = Resources.Load<Sprite>(data.Icon);

            MaybeCreateRarityParticles();
        }

        private void UpdateAvatarUnitLevel()
        {
            var visionMonsterLevel = VisionManager.Instance.GetVisionMonsterLevel(Data, ActIndex);

            if (Data.UnitId > 0)
            {
                var unit = this.avatar.GetComponent<UnitComponent>();
                unit.Owner = Owner.Hostile;
                unit.Level = visionMonsterLevel;

                var health = unit.GetComponent<HealthComponent>();
                health.Health = health.HealthMax;
            }

            if (!string.IsNullOrEmpty(Data.LabelKey))
            {
                SetLabel($"{I18N.Instance.Translate(Data.LabelKey)}\n<size=50%>{I18N.Instance.Translate("ui_level")} {visionMonsterLevel.ToString()}</size>");
            }
        }

        private void OnEnable()
        {
            if (this.avatarActor == null)
            {
                return;
            }

            UpdateAvatarUnitLevel();

            this.avatarActor.Show();
        }

        private void OnDisable()
        {
            if (this.avatarActor == null)
            {
                return;
            }

            this.avatarActor.Hide();
        }

        private void SetLabel(string text)
        {
            if (this.labelText == null)
            {
                return;
            }

            this.labelText.gameObject.SetActive(true);
            this.labelText.text = text;
        }

        public void Terminate()
        {
            if (this.avatarActor != null)
            {
                this.avatarActor.gameObject.Terminate();
            }

            Destroy(gameObject);
        }

        public void Complete()
        {
            this.iconRenderer.enabled = false;
            this.rarityParticles?.DestroyAsVisualEffect();

            if (this.avatarActor == null)
            {
                this.avatar?.gameObject.SetActive(false);
            }
            else
            {
                this.avatarActor?.PlayAnimation("death");
            }

            ChangeSpriteColors(this.normalColor);
            State = VisionViewState.Completed;
        }

        public void Skip()
        {
            this.iconRenderer.enabled = false;
            this.avatar?.gameObject.SetActive(false);
            this.rarityParticles?.SetActive(false);
            ChangeSpriteColors(this.disabledColor);
            State = VisionViewState.Skipped;
        }

        public void Lock()
        {
            this.iconRenderer.enabled = false;
            this.rarityParticles?.SetActive(false);

            if (this.avatarActor == null && this.avatar != null)
            {
                this.avatar.gameObject.SetActive(false);
            }

            ChangeSpriteColors(this.normalColor.With(a: 0.05f));
            State = VisionViewState.Locked;
        }

        public void Reveal()
        {
            this.iconRenderer.enabled = true;
            this.iconRenderer.color = Color.white.With(a: 0.33f);
            this.rarityParticles?.SetActive(true);
            this.avatar?.gameObject.SetActive(true);
            State = VisionViewState.Revealed;
        }

        public void Unlock()
        {
            this.iconRenderer.enabled = true;
            this.rarityParticles?.SetActive(true);
            this.animator.Play("birth");
            this.avatar?.gameObject.SetActive(true);

            ChangeSpriteColors(this.normalColor);
            State = VisionViewState.Unlocked;

            if (string.IsNullOrEmpty(Data.Sound))
            {
                AudioManager.Instance.PlayVisionsEncounterDrop();
            }
            else
            {
                AudioManager.Instance.PlayOneShot(Data.Sound);
            }
        }

        public void SetArrowsEnabled(bool isEnabled)
        {
            SetArrowsEnabled(isEnabled, isEnabled, isEnabled, isEnabled);
        }

        public void SetArrowsEnabled(bool top, bool right, bool bottom, bool left)
        {
            this.arrowTop.gameObject.SetActive(top);
            this.arrowRight.gameObject.SetActive(right);
            this.arrowBottom.gameObject.SetActive(bottom);
            this.arrowLeft.gameObject.SetActive(left);
        }

        private void MaybeCreateRarityParticles()
        {
            GameObject prefab = null;

            switch (Data.RarityId)
            {
                case Constants.ItemRarityIdMagic:
                    prefab = this.magicRarityParticles;
                    break;
                case Constants.ItemRarityIdRare:
                    prefab = this.rareRarityParticles;
                    break;
                case Constants.ItemRarityIdUnique:
                    prefab = this.uniqueRarityParticles;
                    break;
                case Constants.ItemRarityIdLegendary:
                    prefab = this.legendaryRarityParticles;
                    break;
                case Constants.ItemRarityIdMythic:
                    prefab = this.mythicRarityParticles;
                    break;
            }

            if (prefab == null)
            {
                return;
            }

            this.rarityParticles = Instantiate(prefab, transform);
        }

        private bool IsInteractable()
        {
            var isBlocked = UIManager.Instance.IsGameFieldBlockedByUI() ||
                            IsCompleted ||
                            IsSkipped ||
                            IsLocked ||
                            IsRevealed;

            return !isBlocked;
        }

        private void OnMouseEnter()
        {
            var showTooltip = !UIManager.Instance.IsGameFieldBlockedByUI() && (IsInteractable() || IsRevealed) && !Data.IsFinal && !Input.GetMouseButton(0);

            if (showTooltip)
            {
                var description = I18N.Instance.Translate(Data.DescriptionKey);

                if (Data.Sanity > 0)
                {
                    description += $"\n\n{I18N.Instance.Translate("ui_sanity")}: <color=#7800C3>{Data.Sanity}</color>";
                }

                Tooltip.Instance.Show(I18N.Instance.Translate(Data.NameKey), description, Camera.main.WorldToScreenPoint(this.tooltipAnchor.position));
            }

            if (!IsInteractable())
            {
                return;
            }

            IsAnyActiveHovered = true;

            AudioManager.Instance.PlayMouseEnter();
            ChangeSpriteColors(this.hoverColor);
        }

        private void OnMouseExit()
        {
            IsAnyActiveHovered = false;

            Tooltip.Instance.Hide();

            if (!IsInteractable())
            {
                return;
            }

            ChangeSpriteColors(this.normalColor);
        }

        private void OnMouseDown()
        {
            if (!IsInteractable())
            {
                return;
            }

            AudioManager.Instance.PlayMouseClick();
            ChangeSpriteColors(this.pressedColor);
        }

        private void OnMouseUpAsButton()
        {
            if (!IsInteractable())
            {
                return;
            }

            ChangeSpriteColors(this.hoverColor);
            AnyClicked?.Invoke(this);
        }

        private void ChangeSpriteColors(Color color)
        {
            this.frameRenderer.color = color;
            this.iconRenderer.color = color;
        }
    }
}
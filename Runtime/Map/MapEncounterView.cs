using System;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.Map
{
    public class MapEncounterView : MonoBehaviour
    {
        private static readonly int s_IsShaking = Animator.StringToHash("IsShaking");

        public event Action<MapEncounterView>? Clicked;

        public static bool IsAnyActiveHovered;

        public int Index { get; private set; }
        public MapEncounterData Data { get; private set; } = null!;
        public MapEncounterState State { get; private set; } = MapEncounterState.Unlocked;
        public bool IsHidden { get; private set; }
        public bool IsShaking => m_Animator.GetBool(s_IsShaking);

        public bool IsLocked => State == MapEncounterState.Locked;
        public bool IsCompleted => State == MapEncounterState.Completed;
        public bool IsUnlocked => State == MapEncounterState.Unlocked;

        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI m_Label = null!;

        [SerializeField]
        private SpriteRenderer m_FrameRenderer = null!;

        [SerializeField]
        private SpriteRenderer m_IconRenderer = null!;

        [SerializeField]
        private Animator m_Animator = null!;

        [SerializeField]
        private Transform m_Container = null!;

        [SerializeField]
        private Transform m_TooltipAnchor = null!;

        [Header("Colors")]
        [SerializeField]
        private Color m_NormalColor;

        [SerializeField]
        private Color m_HoverColor;

        [SerializeField]
        private Color m_PressedColor;

        [Header("Particles")]
        [SerializeField]
        private GameObject m_MagicRarityParticles = null!;

        [SerializeField]
        private GameObject m_RareRarityParticles = null!;

        [SerializeField]
        private GameObject m_UniqueRarityParticles = null!;

        [SerializeField]
        private GameObject m_LegendaryRarityParticles = null!;

        [SerializeField]
        private GameObject m_MythicRarityParticles = null!;

        [SerializeField]
        private GameObject m_PuffParticles = null!;

        private ActorComponent? m_Actor;
        private GameObject? m_Prefab;
        private GameObject? m_RarityParticles;

        public void Initialize(int index, MapEncounterData data)
        {
            Index = index;
            name = $"Encounter #{data.Id.ToString()} ({data.Type})";
            Data = data;

            MaybeCreateActor();
            MaybeCreateObject();
            MaybeCreateRarityParticles();

            if (data.IsFinal)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            if (string.IsNullOrEmpty(data.LabelKey) == false)
            {
                m_Label.transform.parent.gameObject.SetActive(true);
                m_Label.text = I18N.Instance.Translate(Data.LabelKey);
            }

            m_IconRenderer.enabled = false;
            m_IconRenderer.sprite = Resources.Load<Sprite>(data.Icon);
        }

        public void Terminate()
        {
            m_Actor?.gameObject.Terminate();
        }

        public void Complete()
        {
            m_IconRenderer.enabled = false;
            m_RarityParticles?.DestroyAsVisualEffect();
            m_Actor?.PlayAnimation("death");
            m_Actor?.Model.FadeOut();
            m_Prefab?.SetActive(false);

            m_Label.transform.parent.gameObject.SetActive(false);

            ChangeSpriteColors(new Color(0.2f, 0.2f, 0.2f));
            State = MapEncounterState.Completed;
        }

        public void Lock()
        {
            m_IconRenderer.enabled = false;
            m_RarityParticles?.SetActive(false);
            m_Actor?.gameObject.SetActive(false);
            m_Prefab?.SetActive(false);

            ChangeSpriteColors(m_NormalColor.With(a: 0.05f));
            State = MapEncounterState.Locked;
        }

        public void Reveal()
        {
            m_IconRenderer.enabled = true;
            m_RarityParticles?.SetActive(false);
            m_Actor?.gameObject.SetActive(true);
            m_Prefab?.SetActive(true);

            m_IconRenderer.color = m_NormalColor.With(a: 0.33f);
            State = MapEncounterState.Revealed;
        }

        public void Unlock()
        {
            m_IconRenderer.enabled = true;
            m_RarityParticles?.SetActive(true);
            m_Actor?.gameObject.SetActive(true);
            m_Prefab?.SetActive(true);

            ChangeSpriteColors(m_NormalColor);
            State = MapEncounterState.Unlocked;

            m_Animator.Play("birth");

            if (string.IsNullOrEmpty(Data.Sound))
            {
                AudioManager.Instance.PlayVisionsEncounterDrop();
            }
            else
            {
                AudioManager.Instance.PlayOneShot(Data.Sound);
            }
        }

        public void Shake()
        {
            if (Data.IsFinal)
            {
                return;
            }

            m_Animator.SetBool(s_IsShaking, true);
        }

        public void Hide(bool spawnParticles = true)
        {
            IsHidden = true;

            m_Animator.SetBool(s_IsShaking, false);
            gameObject.SetActive(false);

            if (spawnParticles)
            {
                Instantiate(m_PuffParticles, transform.position, Quaternion.identity).DestroyAsVisualEffect();
            }
        }

        private void OnEnable()
        {
            m_Actor?.Show();
        }

        private void OnDisable()
        {
            m_Actor?.Hide();
        }

        private void MaybeCreateActor()
        {
            if (Data.UnitId == 0)
            {
                return;
            }

            var unit = Container.Instance.Resolve<IUnitRepository>().Find(Data.UnitId)!;
            m_Actor = unit.GetComponent<ActorComponent>();
            m_Actor.transform.parent = m_Container;
            m_Actor.transform.localPosition = Vector3.zero;
            m_Actor.Model.FadeInOnStart = false;
        }

        private void MaybeCreateObject()
        {
            if (string.IsNullOrEmpty(Data.Prefab))
            {
                return;
            }

            m_Prefab = Instantiate(Resources.Load<GameObject>(Data.Prefab), m_Container);
        }

        private void MaybeCreateRarityParticles()
        {
            var prefab = Data.RarityId switch
            {
                Constants.c_ItemRarityIdMagic => m_MagicRarityParticles,
                Constants.c_ItemRarityIdRare => m_RareRarityParticles,
                Constants.c_ItemRarityIdUnique => m_UniqueRarityParticles,
                Constants.c_ItemRarityIdLegendary => m_LegendaryRarityParticles,
                Constants.c_ItemRarityIdMythic => m_MythicRarityParticles,
                _ => null
            };

            if (prefab == null)
            {
                return;
            }

            m_RarityParticles = Instantiate(prefab, m_Container);
        }

        private bool IsInteractable()
        {
            return UIManager.Instance.IsGameFieldBlockedByUI() == false && IsUnlocked && !IsHidden && !IsCompleted;
        }

        private void OnMouseEnter()
        {
            if (IsInteractable() == false)
            {
                return;
            }

            if (Input.GetMouseButton(0) == false)
            {
                if (string.IsNullOrEmpty(Data.DescriptionKey) == false)
                {
                    var description = I18N.Instance.Translate(Data.DescriptionKey);
                    Tooltip.Instance.Show(I18N.Instance.Translate(Data.NameKey), description, Camera.main!.WorldToScreenPoint(m_TooltipAnchor.position));
                }
            }

            IsAnyActiveHovered = true;

            AudioManager.Instance.PlayMouseEnter();
            ChangeSpriteColors(m_HoverColor);
        }

        private void OnMouseExit()
        {
            IsAnyActiveHovered = false;
            Tooltip.Instance.Hide();

            if (IsInteractable())
            {
                ChangeSpriteColors(m_NormalColor);
            }
        }

        private void OnMouseDown()
        {
            if (IsInteractable() == false)
            {
                return;
            }

            Tooltip.Instance.Hide();
            AudioManager.Instance.PlayMouseClick();
            ChangeSpriteColors(m_PressedColor);
        }

        private void OnMouseUpAsButton()
        {
            if (IsInteractable() == false)
            {
                return;
            }

            ChangeSpriteColors(m_HoverColor);
            Clicked?.Invoke(this);
        }

        private void ChangeSpriteColors(Color color)
        {
            m_FrameRenderer.color = color;
            m_IconRenderer.color = color;
        }
    }
}
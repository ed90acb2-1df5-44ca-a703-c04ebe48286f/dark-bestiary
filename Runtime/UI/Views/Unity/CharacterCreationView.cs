using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Events;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class CharacterCreationView : View, ICharacterCreationView
    {
        public event Action Cancel;
        public event Action<int> HairstyleChanged;
        public event Action<int> BeardChanged;
        public event Action<int> HairColorChanged;
        public event Action<int> BeardColorChanged;
        public event Action<int> SkinColorChanged;
        public event Action<Background> BackgroundSelected;
        public event Action<CharacterCreationEventData> Create;

        [SerializeField] private BackgroundRow m_BackgroundRowPrefab;
        [SerializeField] private Transform m_BackgroundRowContainer;
        [SerializeField] private TMP_InputField m_NameInput;
        [SerializeField] private ArrowSelect m_HairSelect;
        [SerializeField] private ArrowSelect m_BeardSelect;
        [SerializeField] private ColorSelect m_HairColorSelect;
        [SerializeField] private ColorSelect m_BeardColorSelect;
        [SerializeField] private ColorSelect m_SkinColorSelect;
        [SerializeField] private Button m_CreateButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Button m_RandomButton;
        [SerializeField] private GameObject m_BackgroundImagePrefab;
        [SerializeField] private SkillSlotView m_SkillPrefab;
        [SerializeField] private Transform m_SkillContainer;

        private readonly List<BackgroundRow> m_BackgroundRows = new();
        private readonly List<SkillSlotView> m_SkillSlotViews = new();

        private BackgroundRow m_SelectedBackgroundRow;
        private GameObject m_BackgroundImage;

        public void Construct(CharacterCreationViewContext context)
        {
            m_BackgroundImage = Instantiate(m_BackgroundImagePrefab);
            // this.backgroundImage.transform.localScale = UIManager.Instance.ViewCanvas.localScale;

            m_NameInput.onEndEdit.AddListener(OnNameInputChanged);
            m_CreateButton.onClick.AddListener(OnCreateButtonClicked);
            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);
            m_RandomButton.onClick.AddListener(OnRandomButtonClicked);

            foreach (var background in context.Backgrounds)
            {
                var backgroundRow = Instantiate(m_BackgroundRowPrefab, m_BackgroundRowContainer);
                backgroundRow.Clicked += OnBackgroundRowClicked;
                backgroundRow.Construct(background);
                m_BackgroundRows.Add(backgroundRow);
            }

            OnBackgroundRowClicked(m_BackgroundRows.First());

            m_HairSelect.Initialize(context.HairstyleCount);
            m_HairSelect.Changed += OnHairSelectChanged;

            m_BeardSelect.Initialize(context.BeardCount);
            m_BeardSelect.Changed += OnBeardSelectChanged;

            m_HairColorSelect.Initialize(context.HairColors);
            m_HairColorSelect.Changed += OnHairColorSelectChanged;

            m_BeardColorSelect.Initialize(context.HairColors);
            m_BeardColorSelect.Changed += OnBeardColorSelectChanged;

            m_SkinColorSelect.Initialize(context.SkinColors);
            m_SkinColorSelect.Changed += OnSkinColorSelectChanged;

            OnRandomButtonClicked();
        }

        protected override void OnTerminate()
        {
            ClearSkillSlots();

            foreach (var backgroundRow in m_BackgroundRows)
            {
                backgroundRow.Clicked -= OnBackgroundRowClicked;
            }

            Destroy(m_BackgroundImage);

            m_NameInput.onEndEdit.RemoveListener(OnNameInputChanged);
            m_CreateButton.onClick.RemoveListener(OnCreateButtonClicked);
            m_CancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            m_RandomButton.onClick.RemoveListener(OnRandomButtonClicked);
        }

        private void OnBackgroundRowClicked(BackgroundRow backgroundRow)
        {
            if (m_SelectedBackgroundRow != null)
            {
                m_SelectedBackgroundRow.Deselect();
            }

            m_SelectedBackgroundRow = backgroundRow;
            m_SelectedBackgroundRow.Select();

            BackgroundSelected?.Invoke(m_SelectedBackgroundRow.Background);
        }

        public void UpdateSkillSlots(List<SkillSlot> slots)
        {
            ClearSkillSlots();

            foreach (var slot in slots)
            {
                var skillSlotView = Instantiate(m_SkillPrefab, m_SkillContainer);
                skillSlotView.ShowParticles = false;
                skillSlotView.Initialize(slot);
                skillSlotView.DisableDrag();
                skillSlotView.HideHotkey();
                m_SkillSlotViews.Add(skillSlotView);
            }
        }

        private void ClearSkillSlots()
        {
            foreach (var skillSlotView in m_SkillSlotViews)
            {
                skillSlotView.Terminate();
                Destroy(skillSlotView.gameObject);
            }

            m_SkillSlotViews.Clear();
        }

        private void OnHairColorSelectChanged(int index)
        {
            HairColorChanged?.Invoke(index);
        }

        private void OnBeardColorSelectChanged(int index)
        {
            BeardColorChanged?.Invoke(index);
        }

        private void OnSkinColorSelectChanged(int index)
        {
            SkinColorChanged?.Invoke(index);
        }

        private void OnHairSelectChanged(int index)
        {
            HairstyleChanged?.Invoke(index);
        }

        private void OnBeardSelectChanged(int index)
        {
            BeardChanged?.Invoke(index);
        }

        private void OnNameInputChanged(string value)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                OnCreateButtonClicked();
            }
        }

        private void OnRandomButtonClicked()
        {
            m_BeardSelect.Random();
            m_HairSelect.Random();

            m_BeardColorSelect.Random();
            m_HairColorSelect.SetColor(m_BeardColorSelect.Color);

            m_SkinColorSelect.Random();
        }

        private void OnCancelButtonClicked()
        {
            Cancel?.Invoke();
        }

        private void OnCreateButtonClicked()
        {
            Create?.Invoke(new CharacterCreationEventData(m_NameInput.text));
        }
    }
}
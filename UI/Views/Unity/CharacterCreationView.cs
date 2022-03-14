using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class CharacterCreationView : View, ICharacterCreationView
    {
        public event Payload Cancel;
        public event Payload<int> HairstyleChanged;
        public event Payload<int> BeardChanged;
        public event Payload<int> HairColorChanged;
        public event Payload<int> BeardColorChanged;
        public event Payload<int> SkinColorChanged;
        public event Payload<Background> BackgroundSelected;
        public event Payload<CharacterCreationEventData> Create;

        [SerializeField] private BackgroundRow backgroundRowPrefab;
        [SerializeField] private Transform backgroundRowContainer;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private ArrowSelect hairSelect;
        [SerializeField] private ArrowSelect beardSelect;
        [SerializeField] private ColorSelect hairColorSelect;
        [SerializeField] private ColorSelect beardColorSelect;
        [SerializeField] private ColorSelect skinColorSelect;
        [SerializeField] private Button createButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button randomButton;
        [SerializeField] private GameObject backgroundImagePrefab;
        [SerializeField] private SkillSlotView skillPrefab;
        [SerializeField] private Transform skillContainer;

        private readonly List<BackgroundRow> backgroundRows = new List<BackgroundRow>();
        private readonly List<SkillSlotView> skillSlotViews = new List<SkillSlotView>();

        private BackgroundRow selectedBackgroundRow;
        private GameObject backgroundImage;

        public void Construct(CharacterCreationViewContext context)
        {
            this.backgroundImage = Instantiate(this.backgroundImagePrefab);
            // this.backgroundImage.transform.localScale = UIManager.Instance.ViewCanvas.localScale;

            this.nameInput.onEndEdit.AddListener(OnNameInputChanged);
            this.createButton.onClick.AddListener(OnCreateButtonClicked);
            this.cancelButton.onClick.AddListener(OnCancelButtonClicked);
            this.randomButton.onClick.AddListener(OnRandomButtonClicked);

            foreach (var background in context.Backgrounds)
            {
                var backgroundRow = Instantiate(this.backgroundRowPrefab, this.backgroundRowContainer);
                backgroundRow.Clicked += OnBackgroundRowClicked;
                backgroundRow.Construct(background);
                this.backgroundRows.Add(backgroundRow);
            }

            OnBackgroundRowClicked(this.backgroundRows.First());

            this.hairSelect.Initialize(context.HairstyleCount);
            this.hairSelect.Changed += OnHairSelectChanged;

            this.beardSelect.Initialize(context.BeardCount);
            this.beardSelect.Changed += OnBeardSelectChanged;

            this.hairColorSelect.Initialize(context.HairColors);
            this.hairColorSelect.Changed += OnHairColorSelectChanged;

            this.beardColorSelect.Initialize(context.HairColors);
            this.beardColorSelect.Changed += OnBeardColorSelectChanged;

            this.skinColorSelect.Initialize(context.SkinColors);
            this.skinColorSelect.Changed += OnSkinColorSelectChanged;

            OnRandomButtonClicked();
        }

        protected override void OnTerminate()
        {
            ClearSkillSlots();

            foreach (var backgroundRow in this.backgroundRows)
            {
                backgroundRow.Clicked -= OnBackgroundRowClicked;
            }

            Destroy(this.backgroundImage);

            this.nameInput.onEndEdit.RemoveListener(OnNameInputChanged);
            this.createButton.onClick.RemoveListener(OnCreateButtonClicked);
            this.cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            this.randomButton.onClick.RemoveListener(OnRandomButtonClicked);
        }

        private void OnBackgroundRowClicked(BackgroundRow backgroundRow)
        {
            if (this.selectedBackgroundRow != null)
            {
                this.selectedBackgroundRow.Deselect();
            }

            this.selectedBackgroundRow = backgroundRow;
            this.selectedBackgroundRow.Select();

            BackgroundSelected?.Invoke(this.selectedBackgroundRow.Background);
        }

        public void UpdateSkillSlots(List<SkillSlot> slots)
        {
            ClearSkillSlots();

            foreach (var slot in slots)
            {
                var skillSlotView = Instantiate(this.skillPrefab, this.skillContainer);
                skillSlotView.ShowParticles = false;
                skillSlotView.Initialize(slot);
                skillSlotView.DisableDrag();
                skillSlotView.HideHotkey();
                this.skillSlotViews.Add(skillSlotView);
            }
        }

        private void ClearSkillSlots()
        {
            foreach (var skillSlotView in this.skillSlotViews)
            {
                skillSlotView.Terminate();
                Destroy(skillSlotView.gameObject);
            }

            this.skillSlotViews.Clear();
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
            this.beardSelect.Random();
            this.hairSelect.Random();

            this.beardColorSelect.Random();
            this.hairColorSelect.SetColor(this.beardColorSelect.Color);

            this.skinColorSelect.Random();
        }

        private void OnCancelButtonClicked()
        {
            Cancel?.Invoke();
        }

        private void OnCreateButtonClicked()
        {
            Create?.Invoke(new CharacterCreationEventData(this.nameInput.text));
        }
    }
}
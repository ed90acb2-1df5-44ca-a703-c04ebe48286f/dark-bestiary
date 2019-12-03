using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
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
        [SerializeField] private Toggle isHardcoreToggle;
        [SerializeField] private Toggle isRandomSkillsToggle;
        [SerializeField] private GameObject backgroundImagePrefab;

        private readonly List<BackgroundRow> backgroundRows = new List<BackgroundRow>();

        private BackgroundRow selectedBackgroundRow;
        private GameObject backgroundImage;

        public void Construct(List<Background> backgrounds,
            List<Color> hairColors, List<Color> skinColors, int hairstyleCount, int beardCount)
        {
            foreach (var background in backgrounds)
            {
                var backgroundRow = Instantiate(this.backgroundRowPrefab, this.backgroundRowContainer);
                backgroundRow.Clicked += OnBackgroundRowClicked;
                backgroundRow.Construct(background);
                this.backgroundRows.Add(backgroundRow);
            }

            OnBackgroundRowClicked(this.backgroundRows.First());

            this.hairSelect.Initialize(hairstyleCount);
            this.hairSelect.Changed += OnHairSelectChanged;

            this.beardSelect.Initialize(beardCount);
            this.beardSelect.Changed += OnBeardSelectChanged;

            this.hairColorSelect.Initialize(hairColors);
            this.hairColorSelect.Changed += OnHairColorSelectChanged;

            this.beardColorSelect.Initialize(hairColors);
            this.beardColorSelect.Changed += OnBeardColorSelectChanged;

            this.skinColorSelect.Initialize(skinColors);
            this.skinColorSelect.Changed += OnSkinColorSelectChanged;
        }

        protected override void OnInitialize()
        {
            this.backgroundImage = Instantiate(this.backgroundImagePrefab);

            this.nameInput.onEndEdit.AddListener(OnNameInputChanged);
            this.createButton.onClick.AddListener(OnCreateButtonClicked);
            this.cancelButton.onClick.AddListener(OnCancelButtonClicked);
            this.randomButton.onClick.AddListener(OnRandomButtonClicked);
        }

        protected override void OnTerminate()
        {
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
            Create?.Invoke(new CharacterCreationEventData(
                this.nameInput.text, this.isHardcoreToggle.isOn, this.isRandomSkillsToggle.isOn));
        }
    }
}
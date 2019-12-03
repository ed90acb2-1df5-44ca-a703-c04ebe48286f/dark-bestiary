using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Dialogues;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary
{
    public class Town : MonoBehaviour
    {
        public event Payload VendorRequested;
        public event Payload CraftRequested;
        public event Payload ForgingRequested;
        public event Payload DismantlingRequested;
        public event Payload SkillVendorRequested;
        public event Payload BestiaryRequested;
        public event Payload RemoveGemsRequested;
        public event Payload SocketingRequested;
        public event Payload ScenariosRequested;
        public event Payload StashRequested;
        public event Payload GambleRequested;
        public event Payload ReliquaryRequested;
        public event Payload AlchemyRequested;
        public event Payload EateryRequested;

        [SerializeField] private Building vendor;
        [SerializeField] private Building arcanist;
        [SerializeField] private Building blacksmith;
        [SerializeField] private Building tavern;
        [SerializeField] private Building map;
        [SerializeField] private Vector2 cameraBounds;
        [SerializeField] private Camera townCamera;

        private IDialogueRepository dialogueRepository;
        private IPhraseDataRepository phraseRepository;

        private void Start()
        {
            this.dialogueRepository = Container.Instance.Resolve<IDialogueRepository>();
            this.phraseRepository = Container.Instance.Resolve<IPhraseDataRepository>();

            this.vendor.Construct(I18N.Instance.Get("ui_vendor"));
            this.vendor.MouseUp += OnVendorMouseUp;

            this.arcanist.Construct(I18N.Instance.Get("ui_arcanist"));
            this.arcanist.MouseUp += OnArcanistMouseUp;

            this.blacksmith.Construct(I18N.Instance.Get("ui_craftsman"));
            this.blacksmith.MouseUp += OnCraftsmanMouseUp;

            this.tavern.Construct(I18N.Instance.Get("ui_tavern"));
            this.tavern.MouseUp += OnTavernMouseUp;

            this.map.Construct(I18N.Instance.Get("ui_command_board"));
            this.map.MouseUp += OnCommandBoardMouseUp;

            Dialogue.AnyDialogueRead += OnAnyDialogueRead;
            OnAnyDialogueRead(null);
        }

        private void OnDestroy()
        {
            Dialogue.AnyDialogueRead -= OnAnyDialogueRead;
        }

        private void OnAnyDialogueRead(Dialogue dialogue)
        {
            var buildings = new Dictionary<Narrator, Building>
            {
                {Narrator.Arcanist, this.arcanist},
                {Narrator.Blacksmith, this.blacksmith},
                {Narrator.Tavern, this.tavern},
                {Narrator.Vendor, this.vendor},
            };

            foreach (var building in buildings)
            {
                UpdateQuestionMarkVisibility(building.Key, building.Value);
            }
        }

        private void OnVendorMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_vendor"))
                .AddSpace();

            MaybeAddPhrase(dialog, Narrator.Vendor);
            MaybeAddDialogues(dialog, Narrator.Vendor, this.vendor);

            dialog
                .AddOption(I18N.Instance.Get("ui_black_market"), () => GambleRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_trade"), () => VendorRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(this.vendor.transform.position));
        }

        private void OnArcanistMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_arcanist"))
                .AddSpace();

            MaybeAddPhrase(dialog, Narrator.Arcanist);
            MaybeAddDialogues(dialog, Narrator.Arcanist, this.arcanist);

            if (CharacterManager.Instance.Character.Data.IsRandomSkills &&
                CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level < 20)
            {
                dialog.AddOption(I18N.Instance.Get("ui_skills") + $" ({I18N.Instance.Get("ui_requires_level")} 20)");
            }
            else
            {
                dialog.AddOption(I18N.Instance.Get("ui_skills"), () => SkillVendorRequested?.Invoke());
            }

            dialog.AddOption(I18N.Instance.Get("ui_bestiary"), () => BestiaryRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_reliquary"), () => ReliquaryRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_alchemy"), () => AlchemyRequested?.Invoke());

            dialog.AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(this.arcanist.transform.position));
        }

        private void OnCraftsmanMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_craftsman"))
                .AddSpace();

            MaybeAddPhrase(dialog, Narrator.Blacksmith);
            MaybeAddDialogues(dialog, Narrator.Blacksmith, this.blacksmith);

            dialog
                .AddOption(I18N.Instance.Get("ui_craft"), () => CraftRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_remove_gems"), () => RemoveGemsRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_forging"), () => ForgingRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_socketing"), () => SocketingRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_dismantling"), () => DismantlingRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(this.blacksmith.transform.position));
        }

        private void OnTavernMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_tavern"))
                .AddSpace();

            MaybeAddPhrase(dialog, Narrator.Tavern);
            MaybeAddDialogues(dialog, Narrator.Tavern, this.tavern);

            dialog
                .AddOption(I18N.Instance.Get("ui_stash"), () => StashRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_eatery"), () => EateryRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(this.tavern.transform.position));
        }

        private void OnCommandBoardMouseUp()
        {
            ScenariosRequested?.Invoke();
        }

        private void MaybeAddPhrase(Dialog dialog, Narrator narrator)
        {
            var phrase = this.phraseRepository.Random(d => d.Narrator == narrator);

            if (phrase == null)
            {
                return;
            }

            dialog
                .AddPhrase(I18N.Instance.Get(phrase.TextKey))
                .AddSpace();
        }

        private void MaybeAddDialogues(Dialog dialog, Narrator narrator, Building building)
        {
            var dialogues = this.dialogueRepository.Find(d => d.Narrator == narrator && d.IsParent)
                .Where(dialogue => dialogue.IsAvailable())
                .ToList();

            if (dialogues.Count == 0)
            {
                return;
            }

            var label = I18N.Instance.Get("ui_talk").ToString();
            var character = CharacterManager.Instance.Character;

            if (!dialogues.All(d => character.IsDialogueRead(d)))
            {
                label += " <color=yellow>(?)</color>";
            }

            dialog.AddOption(label, () =>
            {
                Timer.Instance.WaitForFixedUpdate(() =>
                {
                    var inner = Dialog.Instance.Clear();

                    inner.AddTitle(dialog.Title).AddSpace();

                    foreach (var dialogue in dialogues)
                    {
                        var title = dialogue.Title.ToString();

                        if (!character.Data.ReadDialogues.Contains(dialogue.Id))
                        {
                            title += " <color=yellow>(?)</color>";
                        }

                        inner.AddOption(title, () => DialogueView.Instance.Show(dialogue));
                    }

                    inner.AddSpace().AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red);

                    Dialog.Instance.Show();
                });
            });
        }

        private void UpdateQuestionMarkVisibility(Narrator narrator, Building building)
        {
            var dialogues = this.dialogueRepository.Find(d => d.Narrator == narrator && d.IsParent)
                .Where(dialogue => dialogue.IsAvailable())
                .ToList();

            var character = CharacterManager.Instance.Character;

            if (!dialogues.All(d => character.IsDialogueRead(d)))
            {
                building.ShowQuestionMark();
            }
            else
            {
                building.HideQuestionMark();
            }
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() || UIManager.Instance.ViewStack.Count > 0)
            {
                return;
            }

            var position = new Vector3(
                Input.mousePosition.x / Screen.width * this.cameraBounds.x,
                Input.mousePosition.y / Screen.height * this.cameraBounds.y, -22);

            this.townCamera.transform.position = position;
        }
    }
}
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Dialogues;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TownView : View, ITownView
    {
        public event Payload VendorRequested;
        public event Payload AlchemyRequested;
        public event Payload BlacksmithCraftRequested;
        public event Payload ForgingRequested;
        public event Payload DismantlingRequested;
        public event Payload BestiaryRequested;
        public event Payload RemoveGemsRequested;
        public event Payload SocketingRequested;
        public event Payload ScenariosRequested;
        public event Payload StashRequested;
        public event Payload GambleRequested;
        public event Payload TransmutationRequested;
        public event Payload SphereCraftRequested;
        public event Payload EateryRequested;
        public event Payload ForgottenDepthsRequested;
        public event Payload RunesRequested;
        public event Payload RuneforgeRequested;
        public event Payload LeaderboardRequested;

        [SerializeField] private Building vendor;
        [SerializeField] private Building arcanist;
        [SerializeField] private Building blacksmith;
        [SerializeField] private Building tavern;
        [SerializeField] private Building map;
        [SerializeField] private Building scarecrow;
        [SerializeField] private Building mole;

        private IDialogueRepository dialogueRepository;
        private IPhraseDataRepository phraseRepository;

        protected override void OnInitialize()
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

            if (CharacterManager.Instance.Character.CompletedScenarios.Contains(6))
            {
                this.scarecrow.Construct(I18N.Instance.Get("unit_scarecrow_name"));
                this.scarecrow.MouseUp += OnScarecrowMouseUp;
            }
            else
            {
                this.scarecrow.gameObject.SetActive(false);
            }

            if (Game.IsForgottenDepthsEnabled)
            {
                this.mole.Construct(I18N.Instance.Get("unit_mole_shaman_name"));
                this.mole.MouseUp += OnMoleMouseUp;
            }
            else
            {
                this.mole.gameObject.SetActive(false);
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

        private void OnMoleMouseUp()
        {
            Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("unit_mole_shaman_name"))
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_forgotten_depths"), () => ForgottenDepthsRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_runes"), () => RunesRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_runeforge"), () => RuneforgeRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_leaderboard"), () => LeaderboardRequested?.Invoke())
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_cancel"), null, Color.red)
                .Show(Camera.main.WorldToScreenPoint(this.mole.transform.position));
        }

        private void OnScarecrowMouseUp()
        {
            ConfirmationWindow.Instance.Confirmed += OnScarecrowConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnScarecrowCancelled;
            ConfirmationWindow.Instance.Show(I18N.Instance.Translate("ui_fly_king_invitation"), I18N.Instance.Translate("ui_accept"), true);
        }

        private void OnScarecrowConfirmed()
        {
            OnScarecrowCancelled();

            ScreenFade.Instance.To(() =>
                {
                    Game.Instance.SwitchState(
                        () =>
                        {
                            var character = CharacterManager.Instance.Character;
                            var scenarioId = 74;

                            if (!character.CompletedScenarios.Contains(69))
                            {
                                scenarioId = 69;
                            }
                            else if (!character.CompletedScenarios.Contains(67))
                            {
                                scenarioId = 67;
                            }
                            else if (!character.CompletedScenarios.Contains(70))
                            {
                                scenarioId = 70;
                            }
                            else if (!character.CompletedScenarios.Contains(71))
                            {
                                scenarioId = 71;
                            }
                            else if (!character.CompletedScenarios.Contains(72))
                            {
                                scenarioId = 72;
                            }
                            else if (!character.CompletedScenarios.Contains(73))
                            {
                                scenarioId = 73;
                            }

                            var scenario = Container.Instance.Resolve<IScenarioRepository>().Find(scenarioId);

                            if (scenarioId != 74 || character.CompletedScenarios.Contains(74))
                            {
                                scenario.ChoosableRewards = Container.Instance.Resolve<IItemRepository>()
                                    .Find(x => x.IsEnabled && x.RarityId == Constants.ItemRarityIdLegendary && x.TypeId != Constants.ItemTypeIdGem)
                                    .Where(x => x.IsEquipment)
                                    .OrderBy(x => x.Type.Id)
                                    .ToList();
                            }

                            return new ScenarioGameState(scenario, character);
                        },
                        true
                    );
                }
            );
        }

        private void OnScarecrowCancelled()
        {
            ConfirmationWindow.Instance.Confirmed -= OnScarecrowConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnScarecrowCancelled;
        }

        private void OnArcanistMouseUp()
        {
            var dialog = Dialog.Instance
                .Clear()
                .AddTitle(I18N.Instance.Get("ui_arcanist"))
                .AddSpace();

            MaybeAddPhrase(dialog, Narrator.Arcanist);
            MaybeAddDialogues(dialog, Narrator.Arcanist, this.arcanist);

            dialog
                .AddOption(I18N.Instance.Get("ui_bestiary"), () => BestiaryRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_alchemy"), () => AlchemyRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_transmutation"), () => TransmutationRequested?.Invoke())
                .AddOption(I18N.Instance.Get("ui_sphere_craft"), () => SphereCraftRequested?.Invoke());

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
                .AddOption(I18N.Instance.Get("ui_craft"), () => BlacksmithCraftRequested?.Invoke())
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

            dialog.AddOption(label, () =>
                {
                    Timer.Instance.WaitForFixedUpdate(() =>
                        {
                            var character = CharacterManager.Instance.Character;
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
                        }
                    );
                }
            );
        }
    }
}
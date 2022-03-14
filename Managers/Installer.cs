using DarkBestiary.Achievements;
using DarkBestiary.Analytics;
using DarkBestiary.Audio;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Readers.Json;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Data.Repositories.File;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Leaderboards;
using DarkBestiary.Modifiers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Rewards;
using DarkBestiary.UI.Views;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Managers
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            #if !DISABLESTEAMWORKS
            Container.Bind<IAchievementStorage>().To<SteamAchievementStorage>().AsSingle();
            #else
            Container.Bind<IAchievementStorage>().To<LocalAchievementStorage>().AsSingle();
            #endif

            Container.Bind<StorageId>().FromInstance(new StorageId("Steam")).AsSingle();

            Container.Bind<IFileReader>().To<JsonFileReader>().AsSingle();

            Container.Bind<ICharacterRepository>().To<CharacterFileRepository>().AsSingle();
            Container.Bind<ICharacterDataRepository>().To<CharacterDataFileRepository>().AsSingle();
            Container.Bind<IItemRepository>().To<ItemFileRepository>().AsSingle();
            Container.Bind<IItemTypeRepository>().To<ItemTypeFileRepository>().AsSingle();
            Container.Bind<IRarityRepository>().To<RarityFileRepository>().AsSingle();
            Container.Bind<IItemCategoryRepository>().To<ItemCategoryFileRepository>().AsSingle();
            Container.Bind<ILootDataRepository>().To<LootDataFileRepository>().AsSingle();
            Container.Bind<IBehaviourRepository>().To<BehaviourFileRepository>().AsSingle();
            Container.Bind<IUnitRepository>().To<UnitFileRepository>().AsSingle();
            Container.Bind<IScenarioRepository>().To<ScenarioFileRepository>().AsSingle();
            Container.Bind<IScenarioDataRepository>().To<ScenarioDataFileRepository>().AsSingle();
            Container.Bind<IEffectRepository>().To<EffectFileRepository>().AsSingle();
            Container.Bind<ISkillRepository>().To<SkillFileRepository>().AsSingle();
            Container.Bind<IAttributeRepository>().To<AttributeFileRepository>().AsSingle();
            Container.Bind<IPropertyRepository>().To<PropertyFileRepository>().AsSingle();
            Container.Bind<ICurrencyRepository>().To<CurrencyFileRepository>().AsSingle();
            Container.Bind<IBehaviourTreeRepository>().To<BehaviourTreeFileRepository>().AsSingle();
            Container.Bind<IRecipeRepository>().To<RecipeFileRepository>().AsSingle();
            Container.Bind<IMissileRepository>().To<MissileFileRepository>().AsSingle();
            Container.Bind<IValidatorRepository>().To<ValidatorFileRepository>().AsSingle();
            Container.Bind<IAchievementRepository>().To<AchievementFileRepository>().AsSingle();
            Container.Bind<ITalentRepository>().To<TalentFileRepository>().AsSingle();
            Container.Bind<II18NStringRepository>().To<I18NStringFileRepository>().AsSingle();
            Container.Bind<IRewardRepository>().To<RewardFileRepository>().AsSingle();
            Container.Bind<ISceneRepository>().To<SceneFileRepository>().AsSingle();
            Container.Bind<IItemSetRepository>().To<ItemSetFileRepository>().AsSingle();
            Container.Bind<IBackgroundRepository>().To<BackgroundFileRepository>().AsSingle();
            Container.Bind<IItemModifierRepository>().To<ItemModifierFileRepository>().AsSingle();
            Container.Bind<ISkinRepository>().To<SkinFileRepository>().AsSingle();
            Container.Bind<IArchetypeDataRepository>().To<ArchetypeDataFileRepository>().AsSingle();
            Container.Bind<IUnitDataRepository>().To<UnitDataFileRepository>().AsSingle();
            Container.Bind<ISkillCategoryRepository>().To<SkillCategoryFileRepository>().AsSingle();
            Container.Bind<ITalentCategoryRepository>().To<TalentCategoryFileRepository>().AsSingle();
            Container.Bind<IUnitGroupDataRepository>().To<UnitGroupDataFileRepository>().AsSingle();
            Container.Bind<ISkillSetRepository>().To<SkillSetFileRepository>().AsSingle();
            Container.Bind<IRelicRepository>().To<RelicFileRepository>().AsSingle();
            Container.Bind<IDialogueRepository>().To<DialogueFileRepository>().AsSingle();
            Container.Bind<IPhraseDataRepository>().To<PhraseDataFileRepository>().AsSingle();
            Container.Bind<IMasteryRepository>().To<MasteryFileRepository>().AsSingle();
            Container.Bind<IFoodRepository>().To<FoodFileRepository>().AsSingle();
            Container.Bind<IVisionDataRepository>().To<VisionDataFileRepository>().AsSingle();
            Container.Bind<IBehaviourDataRepository>().To<BehaviourDataFileRepository>().AsSingle();
            Container.Bind<ISpecializationDataRepository>().To<SpecializationDataFileRepository>().AsSingle();
            Container.Bind<IItemDataRepository>().To<ItemDataFileRepository>().AsSingle();
            Container.Bind<ISkillDataRepository>().To<SkillDataFileRepository>().AsSingle();

            Container.Bind<IAudioEngine>().To<FmodAudioEngine>().AsSingle();

            Container.Bind<IAnalyticsService>().FromInstance(new CustomAnalyticsService("https://darkbestiary.com/analytics")).AsSingle();
            Container.Bind<IPathfinder>().FromInstance(Pathfinder.Instance).AsSingle();
            Container.Bind<PathDrawer>().FromInstance(PathDrawer.Instance).AsSingle();
            Container.Bind<CursorManager>().FromInstance(CursorManager.Instance).AsSingle();
            Container.Bind<SelectionManager>().FromInstance(SelectionManager.Instance).AsSingle();
            Container.Bind<Board>().FromInstance(Board.Instance).AsSingle();
            Container.Bind<BoardNavigator>().FromInstance(BoardNavigator.Instance).AsSingle();

            Container.Bind<PropertyModifierFactory>().AsSingle();
            Container.Bind<AttributeModifierFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<LevelupRewardManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Stash>().AsSingle();
            Container.BindInterfacesAndSelfTo<Mailbox>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AchievementManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<TowerManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Interactor>().AsSingle();
            Container.BindInterfacesAndSelfTo<VisionProgression>().AsSingle();
            Container.BindInterfacesAndSelfTo<ForgottenDepthsManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<RandomSkillUnlockManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SteamLeaderboard>().AsSingle();

            // Container.BindInterfacesAndSelfTo<AnalyticsManager>().AsSingle();
            // Container.BindInterfacesAndSelfTo<UnityExceptionManager>().AsSingle();

            Container.Bind<ItemSetMapper>().AsSingle();
            Container.Bind<SkillSetMapper>().AsSingle();
            Container.Bind<SkinMapper>().AsSingle();
            Container.Bind<RewardMapper>().AsSingle();
            Container.Bind<ItemSaveDataMapper>().AsSingle();
            Container.Bind<ItemMapper>().AsSingle();
            Container.Bind<ItemCategoryMapper>().AsSingle();
            Container.Bind<ItemRarityMapper>().AsSingle();
            Container.Bind<ItemTypeMapper>().AsSingle();
            Container.Bind<AchievementMapper>().AsSingle();
            Container.Bind<AttributeMapper>().AsSingle();
            Container.Bind<BehaviourTreeMapper>().AsSingle();
            Container.Bind<BehaviourMapper>().AsSingle();
            Container.Bind<CharacterMapper>().AsSingle();
            Container.Bind<CurrencyMapper>().AsSingle();
            Container.Bind<EffectMapper>().AsSingle();
            Container.Bind<MissileMapper>().AsSingle();
            Container.Bind<PropertyMapper>().AsSingle();
            Container.Bind<RecipeMapper>().AsSingle();
            Container.Bind<ScenarioMapper>().AsSingle();
            Container.Bind<SkillMapper>().AsSingle();
            Container.Bind<UnitMapper>().AsSingle();
            Container.Bind<ValidatorMapper>().AsSingle();
            Container.Bind<TalentMapper>().AsSingle();
            Container.Bind<I18NStringMapper>().AsSingle();
            Container.Bind<BackgroundMapper>().AsSingle();
            Container.Bind<ItemModifierMapper>().AsSingle();
            Container.Bind<SkillCategoryMapper>().AsSingle();
            Container.Bind<TalentCategoryMapper>().AsSingle();
            Container.Bind<EpisodeMapper>().AsSingle();
            Container.Bind<SceneMapper>().AsSingle();
            Container.Bind<RelicMapper>().AsSingle();
            Container.Bind<RelicSaveDataMapper>().AsSingle();
            Container.Bind<DialogueMapper>().AsSingle();
            Container.Bind<MasteryMapper>().AsSingle();
            Container.Bind<MasterySaveDataMapper>().AsSingle();
            Container.Bind<FoodMapper>().AsSingle();

            BindView<ITownView>("Prefabs/Town");

            BindView<IDeveloperConsoleView>("Prefabs/UI/Views/DeveloperConsoleView", UIManager.Instance.OverlayCanvas.transform);

            BindView<ISettingsView>("Prefabs/UI/Views/SettingsView", UIManager.Instance.WidgetCanvas.transform);
            BindView<IMenuView>("Prefabs/UI/Views/MenuView", UIManager.Instance.WidgetCanvas.transform);
            BindView<IKeyBindingsView>("Prefabs/UI/Views/KeyBindingsView", UIManager.Instance.WidgetCanvas.transform);

            BindView<ITargetFrameView>("Prefabs/UI/Views/TargetFrameView", UIManager.Instance.GameplayCanvas.transform);

            BindView<IVisionMapView>("Prefabs/UI/Views/VisionMapView", UIManager.Instance.GameplayCanvasSafeArea.transform);
            BindView<ICharacterUnitFrameView>("Prefabs/UI/Views/CharacterUnitFrameView", UIManager.Instance.GameplayCanvasSafeArea.transform);
            BindView<IActionBarView>("Prefabs/UI/Views/ActionBarView", UIManager.Instance.GameplayCanvasSafeArea.transform);

            BindView<IBestiaryView>("Prefabs/UI/Views/BestiaryView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ILeaderboardView>("Prefabs/UI/Views/LeaderboardView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IRunesView>("Prefabs/UI/Views/RunesView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ITransmutationView>("Prefabs/UI/Views/TransmutationView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IGambleView>("Prefabs/UI/Views/GambleView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IItemForgingView>("Prefabs/UI/Views/ItemForgingView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IEquipmentView>("Prefabs/UI/Views/EquipmentView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ICombatLogView>("Prefabs/UI/Views/CombatLogView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ITalentsView>("Prefabs/UI/Views/TalentsView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVendorView>("Prefabs/UI/Views/VendorView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IStashView>("Prefabs/UI/Views/StashView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IDismantlingView>("Prefabs/UI/Views/DismantlingView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ISpellbookView>("Prefabs/UI/Views/SpellbookView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IBlacksmithView>("Prefabs/UI/Views/BlacksmithView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IAlchemyView>("Prefabs/UI/Views/AlchemyView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IItemUpgradeView>("Prefabs/UI/Views/ItemUpgradeView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ISkillVendorView>("Prefabs/UI/Views/SkillVendorView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVisionSummaryView>("Prefabs/UI/Views/VisionSummaryView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVisionIntroView>("Prefabs/UI/Views/VisionIntroView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVisionMenuView>("Prefabs/UI/Views/VisionMenuView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVisionProgressionView>("Prefabs/UI/Views/VisionProgressionView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IVisionTalentsView>("Prefabs/UI/Views/VisionTalentsView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ISpecializationsView>("Prefabs/UI/Views/SpecializationsView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ISphereCraftView>("Prefabs/UI/Views/SphereCraftView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IForgottenDepthsView>("Prefabs/UI/Views/ForgottenDepthsView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ICommandBoardView>("Prefabs/UI/Views/CommandBoardView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IReliquaryView>("Prefabs/UI/Views/ReliquaryView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IMasteriesView>("Prefabs/UI/Views/MasteriesView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IAchievementsView>("Prefabs/UI/Views/AchievementsView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IAttributesView>("Prefabs/UI/Views/AttributesView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IMailboxView>("Prefabs/UI/Views/MailboxView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IFeedbackView>("Prefabs/UI/Views/FeedbackView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IEateryView>("Prefabs/UI/Views/EateryView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IIntroView>("Prefabs/UI/Views/IntroView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IBuffSelectionView>("Prefabs/UI/Views/TowerBuffSelectionView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ITowerVendorView>("Prefabs/UI/Views/TowerVendorView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<ITowerConfirmationView>("Prefabs/UI/Views/TowerConfirmationView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);
            BindView<IScenarioView>("Prefabs/UI/Views/ScenarioView", UIManager.Instance.ViewCanvasSafeArea.transform.transform);

            BindView<ICharacterSelectionView>("Prefabs/UI/Views/CharacterSelectionView", UIManager.Instance.ViewCanvasSafeArea.transform);
            BindView<ICharacterCreationView>("Prefabs/UI/Views/CharacterCreationView", UIManager.Instance.ViewCanvasSafeArea.transform);
            BindView<ITalkEncounterView>("Prefabs/UI/Views/TalkEncounterView", UIManager.Instance.ViewCanvasSafeArea.transform);
            BindView<INavigationView>("Prefabs/UI/Views/NavigationView", UIManager.Instance.ViewCanvasSafeArea.transform);
            BindView<IMainMenuView>("Prefabs/UI/Views/MainMenuView", UIManager.Instance.ViewCanvasSafeArea.transform);

            DarkBestiary.Container.Instance = new Container(Container);
        }

        private void BindView<T>(string prefab, Transform transform = null) where T : IView
        {
            var concrete = Container.Bind<T>().FromComponentInNewPrefabResource(prefab);

            if (transform != null)
            {
                concrete.UnderTransform(transform);
            }

            concrete.AsTransient().OnInstantiated((context, view) => ((T) view).Hide());
        }
    }
}
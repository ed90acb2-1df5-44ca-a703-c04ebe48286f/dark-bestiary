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
using DarkBestiary.Modifiers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Rewards;
using DarkBestiary.UI.Views;
using Zenject;

#if !DISABLESTEAMWORKS
using Steamworks;
#endif

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

            Container.BindInterfacesAndSelfTo<RandomSkillUnlockManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelupRewardManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Stash>().AsSingle();
            Container.BindInterfacesAndSelfTo<Mailbox>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AchievementManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnalyticsManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<UnityExceptionManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<Interactor>().AsSingle();

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

            Container.Bind<IDeveloperConsoleView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/DeveloperConsoleView")
                .UnderTransform(UIManager.Instance.OverlayCanvas.transform)
                .AsTransient();

            Container.Bind<ICharacterSelectionView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/CharacterSelectionView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ITalkEncounterView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/TalkEncounterView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IEateryView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/EateryView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IBestiaryView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/BestiaryView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IReliquaryView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/ReliquaryView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IMasteriesView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/MasteriesView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IAlchemyView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/AlchemyView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IMailboxView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/MailboxView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IGambleView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/GambleView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IIntroView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/IntroView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IItemForgingView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/ItemForgingView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IFeedbackView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/FeedbackView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IAttributesView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/AttributesView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IMainMenuView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/MainMenuView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IAchievementsView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/AchievementsView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ICharacterCreationView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/CharacterCreationView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ICommandBoardView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/CommandBoardView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ISkillVendorView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/SkillVendorView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IScenarioView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/ScenarioView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<INavigationView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/NavigationView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IEquipmentView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/EquipmentView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ICombatLogView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/CombatLogView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ITargetFrameView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/TargetFrameView")
                .UnderTransform(UIManager.Instance.GameplayCanvas.transform)
                .AsTransient();

            Container.Bind<IActionBarView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/ActionBarView")
                .UnderTransform(UIManager.Instance.GameplayCanvas.transform)
                .AsTransient();

            Container.Bind<ITalentsView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/TalentsView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IVendorView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/VendorView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IStashView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/StashView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IDismantlingView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/DismantlingView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ISpellbookView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/SpellbookView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ISettingsView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/SettingsView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<ICraftView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/CraftView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IItemUpgradeView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/ItemUpgradeView")
                .UnderTransform(UIManager.Instance.ViewCanvas.transform)
                .AsTransient();

            Container.Bind<IMenuView>()
                .FromComponentInNewPrefabResource("Prefabs/UI/Views/MenuView")
                .UnderTransform(UIManager.Instance.WidgetCanvas.transform)
                .AsTransient();

            Container.Bind<Town>()
                .FromComponentInNewPrefabResource("Prefabs/Town")
                .AsTransient();

            DarkBestiary.Container.Instance = new Container(Container);
        }
    }
}
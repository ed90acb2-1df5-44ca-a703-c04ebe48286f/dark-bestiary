using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SpecializationViewController : ViewController<ISpecializationsView>
    {
        private readonly CharacterManager characterManager;
        private readonly ISkillRepository skillRepository;

        public SpecializationViewController(ISpecializationsView view,
            CharacterManager characterManager, ISpecializationDataRepository specializationRepository, ISkillRepository skillRepository) : base(view)
        {
            this.characterManager = characterManager;
            this.skillRepository = skillRepository;
        }

        protected override void OnInitialize()
        {
            var spellbook = this.characterManager.Character.Entity.GetComponent<SpellbookComponent>();
            var specializations = this.characterManager.Character.Entity.GetComponent<SpecializationsComponent>();

            View.Initialize(this.skillRepository, spellbook, specializations);
        }

        protected override void OnTerminate()
        {
            View.Terminate();
        }
    }
}
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.UI.Views
{
    public interface ISpecializationsView : IView
    {
        void Initialize(ISkillRepository skillRepository, SpellbookComponent spellbook, SpecializationsComponent specializations);
    }
}
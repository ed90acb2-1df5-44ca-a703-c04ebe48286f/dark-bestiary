using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Skills;

namespace DarkBestiary.UI.Views
{
    public interface IActionBarView : IView
    {
        event Action<Skill> SkillRemoved;
        event Action<SkillSlot, Skill> SkillPlaced;
        event Action<Skill> SkillClicked;
        event Action EndTurnButtonClicked;
        event Action SwapWeaponButtonClicked;

        void SetPotionBagEnabled(bool isEnabled);

        void CreateSkillSlots(IReadOnlyList<SkillSlot> slots);
        void RemoveSkillSlots();
        void EnableSkillSlots();
        void DisableSkillSlots();

        void AddBehaviour(Behaviour behaviour);
        void RemoveBehaviour(Behaviour behaviour);
        void RemoveBehaviours();

        void SetPoisoned(bool isPoisoned);

        void HighlightSkill(Skill skill);
        void UnhighlightSkill(Skill skill);

        void StartSkillCooldown(Skill skill);
        void UpdateSkillCooldown(Skill skill);
        void StopSkillCooldown(Skill skill);

        void EnableEndTurnButton();
        void DisableEndTurnButton();

        void ShowActionPointUsage(int count);
        void HideActionPointUsage();
        void UpdateActionPointsAmount(float current, float maximum);
        void UpdateRageAmount(float current, float maximum);
        void UpdateHealth(float health, float shield, float maximum);
    }
}
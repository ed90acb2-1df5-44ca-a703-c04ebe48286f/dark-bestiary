using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;

namespace DarkBestiary.UI.Views
{
    public interface ISkillVendorView : IView
    {
        event Payload<Skill> SkillBuyed;

        void Construct(List<SkillSet> sets, List<Skill> skills, List<SkillCategory> categories, List<Currency> currencies);

        void MarkExpensive(Func<Skill, bool> isExpensive);

        void MarkAlreadyKnown(Func<int, bool> isAlreadyKnown);

        void UnlockSkill(Skill skill);
    }
}
﻿using System.Collections.Generic;
using DarkBestiary.Achievements;

namespace DarkBestiary.UI.Views
{
    public interface IAchievementsView : IView, IHideOnEscape
    {
        void Construct(List<Achievement> achievements);
    }
}
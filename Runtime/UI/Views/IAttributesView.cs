using System;
using System.Collections.Generic;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Views
{
    public interface IAttributesView : IView, IHideOnEscape
    {
        event Action<Attribute> AddPoint;
        event Action<Attribute> AddMultiplePoints;
        event Action<Attribute> SubtractPoint;
        event Action<Attribute> SubtractMultiplePoints;
        event Action Reset;

        void Construct(List<Attribute> attributes);
        void SetUnspentPoints(int amount);
        void EnableSpendButtons();
        void DisableSpendButtons();
        void Refresh();
    }
}
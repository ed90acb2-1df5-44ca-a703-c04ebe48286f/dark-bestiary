using System.Collections.Generic;
using DarkBestiary.Attributes;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IAttributesView : IView, IHideOnEscape
    {
        event Payload<Attribute> AddPoint;
        event Payload<Attribute> AddMultiplePoints;
        event Payload<Attribute> SubtractPoint;
        event Payload<Attribute> SubtractMultiplePoints;
        event Payload Reset;

        void Construct(List<Attribute> attributes);
        void SetUnspentPoints(int amount);
        void EnableSpendButtons();
        void DisableSpendButtons();
        void Refresh();
    }
}
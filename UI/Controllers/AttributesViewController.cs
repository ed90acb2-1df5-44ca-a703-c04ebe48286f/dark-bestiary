using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AttributesViewController : ViewController<IAttributesView>
    {
        private readonly AttributesComponent attributes;

        public AttributesViewController(IAttributesView view, CharacterManager characterManager) : base(view)
        {
            this.attributes = characterManager.Character.Entity.GetComponent<AttributesComponent>();
        }

        protected override void OnInitialize()
        {
            this.attributes.PointsChanged += OnAttributePointsChanged;
            this.attributes.AttributeChanged += OnAttributeChanged;

            View.Reset += OnResetPoints;
            View.AddPoint += OnAddPoint;
            View.AddMultiplePoints += OnAddMultiplePoints;
            View.SubtractPoint += OnSubtractPoint;
            View.SubtractMultiplePoints += OnSubtractMultiplePoints;
            View.Construct(this.attributes.Attributes.Values.Where(attribute => attribute.IsPrimary).OrderBy(a => a.Index).ToList());

            OnAttributePointsChanged(this.attributes);
        }

        protected override void OnTerminate()
        {
            this.attributes.PointsChanged -= OnAttributePointsChanged;
            this.attributes.AttributeChanged -= OnAttributeChanged;
        }

        private void OnAttributeChanged(AttributesComponent attributes, Attribute attribute)
        {
            View.Refresh();
        }

        private void UpdateSpendButtons()
        {
            if (this.attributes.Points == 0)
            {
                View.DisableSpendButtons();
            }
            else
            {
                View.EnableSpendButtons();
            }
        }

        private void OnAddPoint(Attribute attribute)
        {
            this.attributes.AddPoint(attribute, 1);
        }

        private void OnAddMultiplePoints(Attribute attribute)
        {
            this.attributes.AddPoint(attribute, 5);
        }

        private void OnSubtractPoint(Attribute attribute)
        {
            this.attributes.SubtractPoint(attribute, 1);
        }

        private void OnSubtractMultiplePoints(Attribute attribute)
        {
            this.attributes.SubtractPoint(attribute, 5);
        }

        private void OnResetPoints()
        {
            this.attributes.ResetPoints();
        }

        private void OnAttributePointsChanged(AttributesComponent attributes)
        {
            View.SetUnspentPoints(attributes.Points);
            UpdateSpendButtons();
        }
    }
}
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AttributesViewController : ViewController<IAttributesView>
    {
        private readonly AttributesComponent m_Attributes;

        public AttributesViewController(IAttributesView view) : base(view)
        {
            m_Attributes = Game.Instance.Character.Entity.GetComponent<AttributesComponent>();
        }

        protected override void OnInitialize()
        {
            m_Attributes.PointsChanged += OnAttributePointsChanged;
            m_Attributes.AttributeChanged += OnAttributeChanged;

            View.Reset += OnResetPoints;
            View.AddPoint += OnAddPoint;
            View.AddMultiplePoints += OnAddMultiplePoints;
            View.SubtractPoint += OnSubtractPoint;
            View.SubtractMultiplePoints += OnSubtractMultiplePoints;
            View.Construct(m_Attributes.Attributes.Values.Where(attribute => attribute.IsPrimary).OrderBy(a => a.Index).ToList());

            OnAttributePointsChanged(m_Attributes);
        }

        protected override void OnTerminate()
        {
            m_Attributes.PointsChanged -= OnAttributePointsChanged;
            m_Attributes.AttributeChanged -= OnAttributeChanged;
        }

        private void OnAttributeChanged(AttributesComponent attributes, Attribute attribute)
        {
            View.Refresh();
        }

        private void UpdateSpendButtons()
        {
            if (m_Attributes.Points == 0)
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
            m_Attributes.AddPoint(attribute, 1);
        }

        private void OnAddMultiplePoints(Attribute attribute)
        {
            m_Attributes.AddPoint(attribute, 10);
        }

        private void OnSubtractPoint(Attribute attribute)
        {
            m_Attributes.SubtractPoint(attribute, 1);
        }

        private void OnSubtractMultiplePoints(Attribute attribute)
        {
            m_Attributes.SubtractPoint(attribute, 10);
        }

        private void OnResetPoints()
        {
            m_Attributes.ResetPoints();
        }

        private void OnAttributePointsChanged(AttributesComponent attributes)
        {
            View.SetUnspentPoints(attributes.Points);
            UpdateSpendButtons();
        }
    }
}
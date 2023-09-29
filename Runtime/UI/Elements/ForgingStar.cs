using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ForgingStar : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_InactiveObject;

        [SerializeField]
        private GameObject m_ActiveObject;

        public void Activate()
        {
            m_ActiveObject.SetActive(true);
        }

        public void Deactivate()
        {
            m_ActiveObject.SetActive(false);
        }
    }
}
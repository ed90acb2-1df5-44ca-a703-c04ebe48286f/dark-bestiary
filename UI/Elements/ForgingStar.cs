using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ForgingStar : MonoBehaviour
    {
        [SerializeField]
        private GameObject inactiveObject;

        [SerializeField]
        private GameObject activeObject;

        public void Activate()
        {
            this.activeObject.SetActive(true);
        }

        public void Deactivate()
        {
            this.activeObject.SetActive(false);
        }
    }
}
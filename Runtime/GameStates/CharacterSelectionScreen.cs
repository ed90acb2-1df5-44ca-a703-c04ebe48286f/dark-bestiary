using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterSelectionScreen : Screen
    {
        private CharacterSelectionViewController m_Controller;

        protected override void OnEnter()
        {
            m_Controller = Container.Instance.Instantiate<CharacterSelectionViewController>();
            m_Controller.Initialize();
            m_Controller.View.Show();
        }

        protected override void OnTick(float delta)
        {
        }

        protected override void OnExit()
        {
            m_Controller.Terminate();
        }
    }
}
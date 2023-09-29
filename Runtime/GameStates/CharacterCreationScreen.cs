using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class CharacterCreationScreen : Screen
    {
        private CharacterCreationViewController m_Controller;

        protected override void OnEnter()
        {
            m_Controller = Container.Instance.Instantiate<CharacterCreationViewController>();
            m_Controller.Initialize();
            m_Controller.View.Show();
        }

        protected override void OnExit()
        {
            m_Controller.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}
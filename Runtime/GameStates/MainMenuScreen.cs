using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class MainMenuScreen : Screen
    {
        private MainMenuViewController m_Controller;

        protected override void OnEnter()
        {
            m_Controller = Container.Instance.Instantiate<MainMenuViewController>();
            m_Controller.Initialize();
            m_Controller.View.Show();

            MusicManager.Instance.Play("event:/Music/MainMenu");
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
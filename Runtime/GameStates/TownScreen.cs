using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class TownScreen : Screen
    {
        protected override void OnEnter()
        {
            Game.Instance.GetController<TownViewController>().View.Show();
            Game.Instance.GetController<NavigationViewController>().View.Show();

            var character = Game.Instance.Character;

            // TODO: Experience recalculation do not trigger achievements
            // Note: this stuff is required to recalculate levels, useful when changing exp formulas
            character.Entity.GetComponent<ExperienceComponent>().Experience.Add(0);
            character.Entity.GetComponent<ReliquaryComponent>().Available.ForEach(r => r.Experience.Add(0));

            MusicManager.Instance.Play("event:/Music/Town");
        }

        protected override void OnExit()
        {
            Game.Instance.GetController<TownViewController>().View.Hide();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}
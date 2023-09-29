namespace DarkBestiary.Scenarios.Encounters
{
    public class EmptyEncounter : Encounter
    {
        protected override void OnStart()
        {
            Complete();
        }
    }
}
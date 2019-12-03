using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;

namespace DarkBestiary.Behaviours
{
    public class MarkerBehaviour : Behaviour
    {
        public MarkerBehaviour(BehaviourData data, List<Validator> validators) : base(data, validators)
        {
        }
    }
}
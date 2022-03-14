using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;

namespace DarkBestiary.Behaviours
{
    public class MarkerBehaviour : Behaviour
    {
        public MarkerBehaviour(BehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
        }
    }
}
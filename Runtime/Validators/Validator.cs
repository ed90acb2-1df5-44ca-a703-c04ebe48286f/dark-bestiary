using UnityEngine;

namespace DarkBestiary.Validators
{
    public abstract class Validator
    {
        public abstract bool Validate(GameObject caster, object target);
    }
}
namespace DarkBestiary.Validators
{
    public class ValidatorWithPurpose
    {
        public Validator Validator { get; }
        public ValidatorPurpose Purpose { get; }

        public ValidatorWithPurpose(Validator validator, ValidatorPurpose purpose)
        {
            Validator = validator;
            Purpose = purpose;
        }
    }
}
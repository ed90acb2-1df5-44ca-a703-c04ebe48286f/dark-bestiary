namespace DarkBestiary.Exceptions
{
    public class CellIsOccupiedException : GameplayException
    {
        public CellIsOccupiedException() : base(I18N.Instance.Get("exception_cell_is_occupied"))
        {
        }
    }
}
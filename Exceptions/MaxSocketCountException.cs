namespace DarkBestiary.Exceptions
{
    public class MaxSocketCountException : GameplayException
    {
        public MaxSocketCountException() : base(I18N.Instance.Get("exception_max_socket_count"))
        {
        }
    }
}
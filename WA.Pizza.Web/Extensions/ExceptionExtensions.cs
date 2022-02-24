namespace WA.Pizza.Web.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<string> GetInnerExceptionMessages(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            var innerException = ex;
            do
            {
                yield return innerException.Message;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}

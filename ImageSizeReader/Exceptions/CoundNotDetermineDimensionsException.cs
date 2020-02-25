namespace ImageSizeReader.Exceptions
{
	public class CoundNotDetermineDimensionsException : BaseException
    {
		public int ErrorCode { get; }

		public CoundNotDetermineDimensionsException(int errorCode)
		{
			ErrorCode = errorCode;
		}
	}
}

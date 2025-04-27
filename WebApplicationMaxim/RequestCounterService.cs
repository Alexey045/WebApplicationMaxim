namespace WebApplicationMaxim
{
	public class RequestCounterService
	{
		private int _currentRequests = 0;

		public int CurrentRequests => _currentRequests;

		public void Increment()
		{
			Interlocked.Increment(ref _currentRequests);
		}

		public void Decrement()
		{
			Interlocked.Decrement(ref _currentRequests);
		}

	}
}

using Microsoft.Extensions.Options;

namespace WebApplicationMaxim
{
	public class RequestLimitMiddleware
	{
		private readonly RequestDelegate next;
		private readonly RequestCounterService counterService;
		private readonly int limit;

		public RequestLimitMiddleware(RequestDelegate next, RequestCounterService counterService, IOptions<AppConfig> configuration)
		{
			this.next = next;
			this.counterService = counterService;
			limit = configuration.Value.Settings!.ParallelLimit;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			counterService.Increment();

			try
			{
				if (counterService.CurrentRequests > limit)
				{
					context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
					await context.Response.WriteAsync("Сервис недоступен: слишком много запросов.");
					return;
				}

				await next(context);
			}
			finally
			{
				counterService.Decrement();
			}
		}

	}
}

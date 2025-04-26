namespace WebApplicationMaxim
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Добавляем привязку конфигурации
			builder.Services.Configure<AppConfig>(builder.Configuration);
			builder.Services.AddSingleton<RequestCounterService>();


			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddHttpClient(); // add client factory

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			app.UseMiddleware<RequestLimitMiddleware>();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();



			app.MapControllers();

			app.Run();
		}
	}
}

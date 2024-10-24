using MeetPoint.API;
using MeetPoint.API.Database;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

// Configuracion para cargar la Seed de Datos
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var loggerFactory = services.GetRequiredService<ILoggerFactory>();

	try
	{
		var context = services.GetRequiredService<MeetPointContext>();
		await MeetPointSeeder.LoadDataAsync(context, loggerFactory);
	}
	catch (Exception e)
	{
		var logger = loggerFactory.CreateLogger<Program>();
		logger.LogError(e, "Error al ejecutar el Seed de datos");
	}
}

app.Run();

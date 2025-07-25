using Kolokwium1Poprawa.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
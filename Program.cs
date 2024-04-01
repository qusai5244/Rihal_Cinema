using Microsoft.EntityFrameworkCore;

using Rihal_Cinema.Data;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Services;
using Rihal_Cinema.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add HttpClient
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICallRihalApiService, CallRihalApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();

        // Call the service to get movie data and seed the database
        var callRihalApiService = services.GetRequiredService<ICallRihalApiService>();
        var seeder = new DatabaseSeeder(context, callRihalApiService);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying database migrations.");
    }
}


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

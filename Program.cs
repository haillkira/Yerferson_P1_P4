using Parcial1_P4_Yerferson.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<AutoresService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var autoresService = scope.ServiceProvider
        .GetRequiredService<AutoresService>();

    await autoresService.InitializeAsync();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
using VegasBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5500", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseCors("AllowLocalhost5500");

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChessHub>("/chesshub");

app.Run();

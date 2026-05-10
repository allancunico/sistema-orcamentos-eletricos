using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Data;
using OrcamentosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serviços
builder.Services.AddScoped<ExcelExportService>();

// CORS para o frontend React
builder.Services.AddCors(opt => opt.AddPolicy("frontend", p =>
    p.WithOrigins("http://localhost:5173", "http://localhost:3000")
     .AllowAnyMethod()
     .AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Aplica migrations e seed automaticamente em desenvolvimento
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();
app.Run();

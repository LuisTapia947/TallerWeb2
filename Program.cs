using Necli.Persistencia;
using Necli.WebApi.Services;
using Microsoft.Extensions.Logging; // Asegura que esta línea está presente

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar logging en la consola
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Registrar los servicios de la aplicación
builder.Services.AddScoped<CuentaRepository>();
builder.Services.AddScoped<GestionarCuentas>();
builder.Services.AddScoped<TransaccionesRepository>(); 
builder.Services.AddScoped<GestionarTransacciones>(); 

var app = builder.Build();

// Configurar el pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

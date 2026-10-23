using Serilog;
using MantenimientoFlota.Api.Exceptions;
using MantenimientoFlota.Application;
using MantenimientoFlota.Api.Common;
using Microsoft.AspNetCore.Mvc;
using MantenimientoFlota.Api.Middleware;
using MantenimientoFlota.Infrastructure;
using Scalar.AspNetCore;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Application.Implementation;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using MantenimientoFlota.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logConfig) => logConfig.ConfigureApiLogging(
    Path.GetFullPath(builder.Configuration["ApiLogs:Directorio"] ?? "logs", builder.Environment.ContentRootPath),
    isDevelopment: builder.Environment.IsDevelopment()));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var imagenRoot = Path.GetFullPath(builder.Configuration["ActivoImagenes:Directorio"] ?? "App_Data/imagenes-activos",
    builder.Environment.ContentRootPath);
builder.Services.AddSingleton(new ActivoImagenStorageOptions(imagenRoot));
builder.Services.AddScoped<IActivoImagenStorage, LocalActivoImagenStorage>();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(ApiErrorFactory.Validation(context.HttpContext, context.ModelState));
});
builder.Services.AddApiDocumentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration,
    builder.Environment.IsDevelopment());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception exception)
    {
        app.Logger.LogCritical("Falló la inicialización de Development. ExceptionType={ExceptionType}",
            exception.GetType().FullName);
        throw new InvalidOperationException("No se pudo inicializar la base de Development. El inicio se ha detenido.");
    }
}

app.UseApiRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler(new ExceptionHandlerOptions { SuppressDiagnosticsCallback = _ => true });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Mantenimiento de Flota API")
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch));
}

app.UseHttpsRedirection();

Directory.CreateDirectory(imagenRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagenRoot),
    RequestPath = LocalActivoImagenStorage.RequestPath,
    OnPrepareResponse = context => context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff"
});

app.UseAuthorization();

app.MapControllers();

app.Run();

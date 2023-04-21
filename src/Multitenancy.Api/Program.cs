using Multitenancy.Api;
using Multitenancy.Api.Middlewares;
using Multitenancy.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddLogging(configuration);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerLayer();

builder.Services.ConfigureApplicationOptions(configuration);
builder.Services.AddDbContexts(configuration);
builder.Services.AddServicesLayer();
builder.Services.AddAuthenticationLayer(configuration);

var app = builder.Build();

await app.Services.MigrateMasterDbContextAsync();
await app.Services.MigrateTenantDbContextsAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantResolverMiddleware>();

app.MapControllers();

app.Run();

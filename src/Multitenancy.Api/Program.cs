using Multitenancy.Api;
using Multitenancy.Api.Middlewares;
using Multitenancy.Services;

var MyAllowSpecificOrigins = "MultitenancyOrigins";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddLogging(configuration);
builder.Services.AddControllers().AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerLayer();

builder.Services.AddMiddlewares();
builder.Services.ConfigureApplicationOptions(configuration);
builder.Services.AddDbContexts(configuration);
builder.Services.AddRedis(configuration);
builder.Services.AddServicesLayer();
builder.Services.AddAuthenticationLayer(configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});

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

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
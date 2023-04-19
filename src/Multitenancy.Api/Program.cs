using Multitenancy.Api;
using Multitenancy.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.AddLogging(configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerLayer();

builder.Services.ConfigureApplicationOptions(configuration);
builder.Services.AddDbContexts(configuration);
builder.Services.AddServicesLayer();
builder.Services.AddAuthenticationLayer(configuration);

var app = builder.Build();

await app.Services.MigrateMasterDbContextAsync();
await app.Services.MigrateTenantDbContextsAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

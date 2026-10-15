using Core.Data;                    
using Core.ScoringConfiguration;    

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string? connectionString = builder.Configuration.GetConnectionString("OraEvents");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'OraEvents' is missing in appsettings.json.");
}

builder.Services.AddDataInfrastructure(connectionString);

builder.Services.AddScoringConfigurationWorkflow();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
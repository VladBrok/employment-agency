using EmploymentAgency.Helpers;
using EmploymentAgency.Models;
using EmploymentAgency.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(ConfigureJson);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AddCustomDependencies(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

/**
    * Data validation
    * Special queries
    
    * Timeouts and error handling
    * Security
*/

void ConfigureJson(JsonOptions options)
{
    var o = options.JsonSerializerOptions;
    o.Converters.Add(new DateOnlyJsonConverter());
    o.Converters.Add(new IdentifiableConverterFactory());
}

void AddCustomDependencies(IServiceCollection services)
{
    var settings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build()
        .GetSection("Settings")
        .Get<Settings>();

    services.AddSingleton(settings);
    services.AddDbContext<EmploymentAgencyContext>();

    services.AddScoped(p => new Service<Seeker>(GetDbContext(p), c => c.Seekers));
    services.AddScoped(p => new Service<Address>(GetDbContext(p), c => c.Addresses));
    services.AddScoped(p => new Service<Application>(GetDbContext(p), c => c.Applications));
    services.AddScoped(p => new Service<ChangeLog>(GetDbContext(p), c => c.ChangeLogs));
    services.AddScoped(p => new Service<District>(GetDbContext(p), c => c.Districts));
    services.AddScoped(p => new Service<Employer>(GetDbContext(p), c => c.Employers));
    services.AddScoped(p => new Service<EmploymentType>(GetDbContext(p), c => c.EmploymentTypes));
    services.AddScoped(p => new Service<Position>(GetDbContext(p), c => c.Positions));
    services.AddScoped(p => new Service<Property>(GetDbContext(p), c => c.Properties));
    services.AddScoped(p => new Service<Status>(GetDbContext(p), c => c.Statuses));
    services.AddScoped(p => new Service<Street>(GetDbContext(p), c => c.Streets));
    services.AddScoped(p => new Service<Vacancy>(GetDbContext(p), c => c.Vacancies));
}

EmploymentAgencyContext GetDbContext(IServiceProvider provider) =>
    provider.GetService<EmploymentAgencyContext>()!;

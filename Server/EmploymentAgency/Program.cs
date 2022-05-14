using System.Text.Json.Serialization;
using EmploymentAgency.Helpers;
using EmploymentAgency.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
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
    * Send data by pages
    * Special queries
    * Data validation
    * Searching ?
    * Timeouts and error handling
    * Security
    * await using ?
*/
void AddCustomDependencies(IServiceCollection services)
{
    var settings = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build()
        .GetSection("Settings")
        .Get<Settings>();

    services.AddSingleton(new Service<Seeker>(settings, x => x.Seekers));
    services.AddSingleton(new Service<Address>(settings, x => x.Addresses));
    services.AddSingleton(new Service<Application>(settings, x => x.Applications));
    services.AddSingleton(new Service<ChangeLog>(settings, x => x.ChangeLogs));
    services.AddSingleton(new Service<District>(settings, x => x.Districts));
    services.AddSingleton(new Service<Employer>(settings, x => x.Employers));
    services.AddSingleton(new Service<EmploymentType>(settings, x => x.EmploymentTypes));
    services.AddSingleton(new Service<Position>(settings, x => x.Positions));
    services.AddSingleton(new Service<Property>(settings, x => x.Properties));
    services.AddSingleton(new Service<Status>(settings, x => x.Statuses));
    services.AddSingleton(new Service<Street>(settings, x => x.Streets));
    services.AddSingleton(new Service<Vacancy>(settings, x => x.Vacancies));
}

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Temp;

Settings settings = await LoadSettingsAsync();
var postgres = new PostgreSql(settings.ConnectionString);
await postgres.ExecuteReaderAsync("select * from seekers order by 1, 2, 3 limit 10");
// await postgres.ExecuteReaderAsync("SELECT * FROM get_seekers_in_district('Ленинский') LIMIT 5;");
// await postgres.ExecuteReaderAsync("insert into positions values(default, ");

async Task<Settings> LoadSettingsAsync()
{
    using var stream = File.OpenRead("appSettings.json");
    return (await JsonSerializer.DeserializeAsync<Settings>(stream))!;
}

public class PostgreSql
{
    private readonly string _connection;

    public PostgreSql(string connection)
    {
        _connection = connection;
    }

    public async Task ExecuteReaderAsync(string command)
    {
        await using var conn = new NpgsqlConnection(_connection);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(command, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            int i = 0;
            foreach (var value in values.Select(x => x.ToString()))
            {
                Console.WriteLine(reader.GetName(i++) + ": " + value);
            }
            Console.WriteLine();
        }
    }
}


// var s = new Service<Address>(settings, context => context.Addresses);
// Address? a = await s.ReadAsync(1);
// Console.WriteLine(a?.BuildingNumber);

// var s = new Service<Seeker>(settings, context => context.Seekers);
// var seeker = new Seeker()
// {
//     Id = 2,
//     StatusId = 1,
//     AddressId = 1,
//     SpecialityId = 1,
//     LastName = "TEST",
//     FirstName = "BRAVE",
//     Birthday = new DateOnly(2000, 1, 1),
//     RegistrationCity = "Uganda",
//     Recommended = true,
//     Pol = true,
// };
// await s.UpdateAsync(seeker.Id, seeker);
// Console.WriteLine((await s.ReadAsync(seeker.Id))?.FirstName);

// public class SeekerService
// {
//     private readonly Settings _settings;

//     public SeekerService(Settings settings)
//     {
//         _settings = settings;
//     }

//     public async Task CreateAsync(Seeker entity)
//     {
//         using var context = new EmploymentAgencyContext(_settings);

//         entity.Id = default;
//         await context.Seekers.AddAsync(entity);

//         await context.SaveChangesAsync();
//     }

//     public async Task<Seeker?> ReadAsync(int id)
//     {
//         using var context = new EmploymentAgencyContext(_settings);

//         return await context.Seekers.FirstOrDefaultAsync(x => x.Id == id);
//     }

//     public async Task<IEnumerable<Seeker>> ReadAsync()
//     {
//         using var context = new EmploymentAgencyContext(_settings);

//         return await context.Seekers.ToListAsync();
//     }

//     public async Task<bool> UpdateAsync(int id, Seeker entity)
//     {
//         using var context = new EmploymentAgencyContext(_settings);

//         var toUpdate = await context.Seekers.FirstOrDefaultAsync(x => x.Id == id);
//         if (toUpdate is null)
//         {
//             return false;
//         }

//         entity.Id = id;
//         context.Entry(toUpdate).CurrentValues.SetValues(entity);

//         await context.SaveChangesAsync();
//         return true;
//     }

//     public async Task<bool> DeleteAsync(int id)
//     {
//         using var context = new EmploymentAgencyContext(_settings);

//         var entity = await context.Seekers.FirstOrDefaultAsync(x => x.Id == id);
//         if (entity is null)
//         {
//             return false; // Not found
//         }

//         context.Seekers.Remove(entity);

//         context.ChangeTracker.DetectChanges();
//         Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

//         await context.SaveChangesAsync();
//         return true; // No content
//     }
// }


// public class PostgreSql
// {
//     private readonly string _connection;

//     public PostgreSql(string connection)
//     {
//         _connection = connection;
//     }

//     public async Task ExecuteReaderAsync(string command)
//     {
//         await using var conn = new NpgsqlConnection(_connection);
//         await conn.OpenAsync();

//         await using var cmd = new NpgsqlCommand(command, conn);
//         await using var reader = await cmd.ExecuteReaderAsync();

//         while (await reader.ReadAsync())
//         {
//             var values = new object[reader.FieldCount];
//             reader.GetValues(values);
//             foreach (var value in values.Select(x => x.ToString()))
//             {
//                 Console.Write(value + " ");
//             }
//             Console.WriteLine();
//         }
//     }
// }

// bool success = await s.DeleteAsync(7398);
// Console.WriteLine($"{(success ? "D" : "Not d")}eleted.");

// await new EmploymentAgencyContext(settings)
//     .Applications
//     .Include(x => x.Seeker)
//     .Take(5)
//     .ForEachAsync(x => Console.WriteLine($"Seeker: {x.Seeker?.Id}, Application: {x.Id}"));

// using var context = new EmploymentAgencyContext(); // Lifetime is short (1 query - 1 connection)
// Update
// context.Entry(entity).CurrentValues.SetValues(newEntity);

// var clock = Stopwatch.StartNew();
// string json = JsonSerializer.Serialize(
//     context.Applications.Take(5),
//     new JsonSerializerOptions
//     {
//         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
//     });
// clock.Stop();
// Console.WriteLine($"Completed in {clock.ElapsedMilliseconds} ms. Result:\n{json}\n");

// await context.Seekers
//     .FromSqlRaw("select * from seekers") // mb no need for this
//     .Take(1)
//     .ForEachAsync(x => Console.WriteLine(x.LastName));
// Console.WriteLine();

// await context.Seekers
//     .Skip(5)
//     .Take(3)
//     .Include(x => x.Speciality)
//     .OrderBy(x => x.Speciality.PositionName)
//     .ForEachAsync(x => Console.WriteLine(x.Speciality.PositionName));

// await context.SaveChangesAsync(); // ???

// var p = new PostgreSql(connectionString);
// await p.ExecuteReaderAsync("select * from seekers");

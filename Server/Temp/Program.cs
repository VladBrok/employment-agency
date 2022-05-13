﻿using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Temp;

/**
    * Connection string
    * CRUD operations
    * Send data by pages
    * Data validation
    * Searching ?
    * Cascade deletion
    * Timeouts and error handling
    * Security
*/

Settings? settings = null;
using (var reader = new StreamReader("appSettings.json"))
{
    string json = await reader.ReadToEndAsync();
    settings = JsonSerializer.Deserialize<Settings>(json, new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    });
}

var s = new SeekerService(settings);
bool success = await s.DeleteAsync(7398);
Console.WriteLine($"{(success ? "D" : "Not d")}eleted.");
await new EmploymentAgencyContext(settings)
    .Applications
    .Include(x => x.Seeker)
    .Take(5)
    .ForEachAsync(x => Console.WriteLine($"Seeker: {x.Seeker?.Id}, Application: {x.Id}"));

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

public class SeekerService
{
    private readonly Settings _settings;

    public SeekerService(Settings settings)
    {
        _settings = settings;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = new EmploymentAgencyContext(_settings);
        var entity = await context.Seekers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        context.Seekers.Remove(entity);

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        await context.SaveChangesAsync();
        return true;
    }
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
            foreach (var value in values.Select(x => x.ToString()))
            {
                Console.Write(value + " ");
            }
            Console.WriteLine();
        }
    }
}

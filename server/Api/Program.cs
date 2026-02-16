using Api;
using Efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// instantiate app options
var appOptions = builder.Services.AddAppOptions(builder.Configuration);

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});

// Add services to the container.
var app = builder.Build();

//basic api routes 
app.MapGet("/", (
    [FromServices]IOptionsMonitor<AppOptions> optionsMonitor,    
    [FromServices]MyDbContext dbContext
) =>
{
    var objects = dbContext.Notes.ToList();
    return objects;
});

app.MapGet("/AddNote", ([FromServices] MyDbContext dbContext) =>
{
    var myNote = new Note()
    {
        Id = Guid.NewGuid().ToString(),
        Title = "First Note 2",
        Description = "First Note Description 2",
        Priority = 2,
        IsComplete = false
    };
    dbContext.Notes.Add(myNote);
    dbContext.SaveChanges();
});

app.Run();

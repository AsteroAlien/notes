using Efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql();
});

var app = builder.Build();

//basic api routes 
app.MapGet("/", ([FromServices]MyDbContext dbContext) =>
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

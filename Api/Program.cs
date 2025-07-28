using System.Reflection;
using Api.Common.ExceptionHandlers;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter(new DependencyContextAssemblyCatalog(Assembly.GetExecutingAssembly()));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler(_ => { });
app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
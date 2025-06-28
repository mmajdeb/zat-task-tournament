using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using TournamentManagerTask.Api.Middlewares;
using TournamentManagerTask.Infrastructure;
using TournamentManagerTask.Application;
using TournamentManagerTask.Api.Extensions;
using TournamentManagerTask.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddDefaultLogging();

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwaggerUI();
}

app.UseRequestLogging();
app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();